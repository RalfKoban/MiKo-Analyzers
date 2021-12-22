using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4004_CodeFixProvider)), Shared]
    public sealed class MiKo_4004_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4004_DisposeMethodsOrderedBeforeOtherMethodsAnalyzer.Id;

        protected override string Title => Resources.MiKo_4004_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var typeSyntax = syntax.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().First();

            var updatedTypeSyntax = GetUpdatedTypeSyntax(document, typeSyntax, syntax, diagnostic);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var disposeMethod = (MethodDeclarationSyntax)syntax;
            var targetMethod = FindTargetMethod(document, typeSyntax, disposeMethod);

            var disposeAnnotation = new SyntaxAnnotation();
            var targetAnnotation = new SyntaxAnnotation();

            var modifiedType = typeSyntax.ReplaceNodes(
                                                       new[] { targetMethod, disposeMethod },
                                                       (original, rewritten) =>
                                                           {
                                                               if (original == targetMethod)
                                                               {
                                                                   return targetMethod.WithAnnotation(targetAnnotation);
                                                               }

                                                               if (original == disposeMethod)
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

            return modifiedType.InsertNodeBefore(annotatedTargetMethod, disposeMethod);
        }

        private static MethodDeclarationSyntax FindTargetMethod(Document document, BaseTypeDeclarationSyntax typeSyntax, MethodDeclarationSyntax disposeMethod)
        {
            var methodSymbol = (IMethodSymbol)GetSymbol(document, disposeMethod);
            var typeSymbol = (INamedTypeSymbol)GetSymbol(document, typeSyntax);

            var methods = typeSymbol.GetMethods().Except(new[] { methodSymbol }).ToList();

            var method = methods.FirstOrDefault(_ => _.DeclaredAccessibility == methodSymbol.DeclaredAccessibility && _.IsStatic is false);
            if (method != null)
            {
                return (MethodDeclarationSyntax)method.GetSyntax();
            }

            // TODO: RKN Fix me (find better node)
            return typeSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
        }
    }
}