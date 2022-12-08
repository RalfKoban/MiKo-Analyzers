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

        protected override SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic issue)
        {
            var typeSyntax = syntax.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();

            var updatedTypeSyntax = GetUpdatedTypeSyntax(context, typeSyntax, syntax, issue);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected override SyntaxNode GetUpdatedTypeSyntax(CodeFixContext context, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var disposeMethod = (MethodDeclarationSyntax)syntax;
            var targetMethod = FindTargetMethod(context, typeSyntax, disposeMethod);

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

        private static MethodDeclarationSyntax FindTargetMethod(CodeFixContext context, BaseTypeDeclarationSyntax typeSyntax, MethodDeclarationSyntax disposeMethod)
        {
            var methodSymbol = (IMethodSymbol)GetSymbol(context, disposeMethod);
            var typeSymbol = (INamedTypeSymbol)GetSymbol(context, typeSyntax);

            var methods = typeSymbol.GetMethods().Except(new[] { methodSymbol }).ToList();

            var method = methods.FirstOrDefault(_ => _.DeclaredAccessibility == methodSymbol.DeclaredAccessibility && _.IsStatic is false);

            if (method != null)
            {
                return (MethodDeclarationSyntax)method.GetSyntax();
            }

            // TODO: RKN Fix me (find better node)
            return typeSyntax.DescendantNodes<MethodDeclarationSyntax>().First();
        }
    }
}