using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class DisposeOrderingCodeFixProvider : OrderingCodeFixProvider
    {
        protected const string DisposeAnnotationKind = "dispose method";

        protected static BaseTypeDeclarationSyntax MoveRegionsWithDisposeAnnotation(BaseTypeDeclarationSyntax typeSyntax, SyntaxNode originalDisposeMethod)
        {
            var disposeMethod = typeSyntax.GetAnnotatedNodes(DisposeAnnotationKind).First();

            if (originalDisposeMethod.IsOnlyNodeInsideRegion())
            {
                return MoveCompleteRegion(typeSyntax, disposeMethod, DisposeAnnotationKind);
            }

            // seems like other methods or stuff are inside the "#region", so we do not move the "#endregion" around, but maybe the "#region"
            if (disposeMethod.NextSibling() is MethodDeclarationSyntax method && method.TryGetRegionDirective(out var regionDirective))
            {
                return MoveRegion(typeSyntax, regionDirective, method, disposeMethod);
            }

            return typeSyntax;
        }

        private static BaseTypeDeclarationSyntax MoveRegion(BaseTypeDeclarationSyntax typeSyntax, IStructuredTriviaSyntax regionDirective, SyntaxNode method, SyntaxNode disposeMethod)
        {
            var triviaToRemove = GetTriviaToRemove(regionDirective.ParentTrivia);

            // add an additional line break here because the "#region" should be followed by an empty line
            var triviaToAdd = GetTriviaToAdd(triviaToRemove, SyntaxFactory.CarriageReturnLineFeed);

            return typeSyntax.ReplaceNodes(
                                       new[] { method, disposeMethod },
                                       (original, rewritten) =>
                                                               {
                                                                   if (rewritten.IsEquivalentTo(method))
                                                                   {
                                                                       return rewritten.Without(triviaToRemove);
                                                                   }

                                                                   if (rewritten.IsEquivalentTo(disposeMethod))
                                                                   {
                                                                       return rewritten.WithFirstLeadingTrivia(triviaToAdd);
                                                                   }

                                                                   return original;
                                                               });
        }
    }
}