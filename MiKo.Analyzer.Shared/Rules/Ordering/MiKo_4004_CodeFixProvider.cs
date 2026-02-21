using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4004_CodeFixProvider)), Shared]
    public sealed class MiKo_4004_CodeFixProvider : DisposeOrderingCodeFixProvider
    {
        private const string TargetAnnotationKind = "target method";

        public override string FixableDiagnosticId => "MiKo_4004";

        protected override async Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var typeSyntax = syntax.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();

            var updatedTypeSyntax = await GetUpdatedTypeSyntaxAsync(document, typeSyntax, syntax, issue, cancellationToken).ConfigureAwait(false);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected override async Task<SyntaxNode> GetUpdatedTypeSyntaxAsync(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var disposeMethod = (MethodDeclarationSyntax)syntax;

            var targetMethod = await FindTargetMethodAsync(typeSyntax, disposeMethod, document, cancellationToken).ConfigureAwait(false);

            var disposeAnnotation = new SyntaxAnnotation(DisposeAnnotationKind);
            var targetAnnotation = new SyntaxAnnotation(TargetAnnotationKind);

            var modifiedType = typeSyntax.ReplaceNodes(
                                                   new[] { targetMethod, disposeMethod },
                                                   (original, rewritten) =>
                                                                           {
                                                                               if (rewritten.IsEquivalentTo(targetMethod))
                                                                               {
                                                                                   return targetMethod.WithAnnotation(targetAnnotation);
                                                                               }

                                                                               if (rewritten.IsEquivalentTo(disposeMethod))
                                                                               {
                                                                                   return disposeMethod.WithAnnotation(disposeAnnotation);
                                                                               }

                                                                               return original;
                                                                           });

            // remove dispose method from modified type to place it at correct location
            var annotatedDisposeMethod = modifiedType.GetAnnotatedNodes(disposeAnnotation).OfType<MethodDeclarationSyntax>().First();
            modifiedType = modifiedType.RemoveNodeAndAdjustOpenCloseBraces(annotatedDisposeMethod);

            // find target method to insert dispose method before that
            var annotatedTargetMethod = modifiedType.GetAnnotatedNodes(targetAnnotation).OfType<MethodDeclarationSyntax>().First();

            return MoveRegionsWithDisposeAnnotation(modifiedType.InsertNodeBefore(annotatedTargetMethod, annotatedDisposeMethod), disposeMethod);
        }

        private static async Task<MethodDeclarationSyntax> FindTargetMethodAsync(BaseTypeDeclarationSyntax typeSyntax, MethodDeclarationSyntax disposeMethod, Document document, CancellationToken cancellationToken)
        {
            var methodSymbol = (IMethodSymbol)await disposeMethod.GetSymbolAsync(document, cancellationToken).ConfigureAwait(false);
            var typeSymbol = (INamedTypeSymbol)await typeSyntax.GetSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            var methods = typeSymbol.GetMethods(MethodKind.Ordinary).Except(methodSymbol);
            var method = methods.FirstOrDefault(_ => _.DeclaredAccessibility == methodSymbol.DeclaredAccessibility && _.IsStatic is false);

            if (method != null)
            {
                return (MethodDeclarationSyntax)method.GetSyntax();
            }

            // TODO: RKN Fix me (find better node)
            return typeSyntax.FirstDescendant<MethodDeclarationSyntax>();
        }
    }
}