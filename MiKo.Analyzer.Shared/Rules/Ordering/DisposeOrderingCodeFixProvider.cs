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
                return MoveWithCompleteRegion(typeSyntax, disposeMethod);
            }

            // seems like other methods or stuff are inside the "#region", so we do not move the "#endregion" around, but maybe the "#region"
            if (disposeMethod.NextSibling() is MethodDeclarationSyntax method && method.TryGetRegionDirective(out var regionDirective))
            {
                return MoveWithRegion(typeSyntax, regionDirective, method, disposeMethod);
            }

            return typeSyntax;
        }

        private static BaseTypeDeclarationSyntax MoveWithCompleteRegion(BaseTypeDeclarationSyntax typeSyntax, SyntaxNode disposeMethod)
        {
            if (disposeMethod.TryGetRegionDirective(out var regionTrivia))
            {
                var relatedDirectives = regionTrivia.GetRelatedDirectives();

                if (relatedDirectives.Count == 2)
                {
                    var endRegionTrivia = relatedDirectives[1].ParentTrivia;
                    var parent = endRegionTrivia.Token.Parent;

                    if (parent != null)
                    {
                        var triviaToRemove = GetTriviaToRemove(endRegionTrivia);

                        // add an additional trivia here because the "#endregion" should be followed by an empty line
                        var triviaToAdd = GetTriviaToAdd(triviaToRemove, SyntaxFactory.EndOfLine(string.Empty));

                        if (typeSyntax.IsEquivalentTo(parent))
                        {
                            // seems like "#endregion" was at the very end of the type, so remove it
                            var syntax = typeSyntax.Without(triviaToRemove);

                            // and add "#endregion" to the trailing of the dispose method
                            disposeMethod = syntax.GetAnnotatedNodes(DisposeAnnotationKind).First();

                            return syntax.ReplaceNode(disposeMethod, disposeMethod.WithAdditionalTrailingTrivia(triviaToAdd));
                        }

                        return typeSyntax.ReplaceNodes(
                                                   new[] { parent, disposeMethod },
                                                   (original, rewritten) =>
                                                                           {
                                                                               if (rewritten.IsEquivalentTo(parent))
                                                                               {
                                                                                   return rewritten.Without(triviaToRemove);
                                                                               }

                                                                               if (rewritten.IsEquivalentTo(disposeMethod))
                                                                               {
                                                                                   return rewritten.WithAdditionalTrailingTrivia(triviaToAdd);
                                                                               }

                                                                               return original;
                                                                           });
                    }
                }
            }

            // should never happen as we have only a single region
            return typeSyntax;
        }

        private static BaseTypeDeclarationSyntax MoveWithRegion(BaseTypeDeclarationSyntax typeSyntax, IStructuredTriviaSyntax regionDirective, SyntaxNode method, SyntaxNode disposeMethod)
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

        private static SyntaxTrivia[] GetTriviaToRemove(SyntaxTrivia trivia)
        {
            var result = Enumerable.Empty<SyntaxTrivia>()
                                   .Concat(trivia.PreviousSiblings(2))
                                   .Concat(new[] { trivia })
                                   .ToArray();

            return result;
        }

        private static SyntaxTrivia[] GetTriviaToAdd(SyntaxTrivia[] triviaToRemove, SyntaxTrivia additionalTrivia)
        {
            var result = new SyntaxTrivia[triviaToRemove.Length + 1];
            result[triviaToRemove.Length] = additionalTrivia;

            triviaToRemove.CopyTo(result, 0);

            return result;
        }
    }
}