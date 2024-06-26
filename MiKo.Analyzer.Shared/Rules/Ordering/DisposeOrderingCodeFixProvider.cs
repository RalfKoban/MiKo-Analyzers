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
                return MoveCompleteRegion(typeSyntax, disposeMethod);
            }

            // seems like other methods or stuff are inside the "#region", so we do not move the "#endregion" around, but maybe the "#region"
            if (disposeMethod.NextSibling() is MethodDeclarationSyntax method && method.TryGetRegionDirective(out var regionDirective))
            {
                return MoveRegion(typeSyntax, regionDirective, method, disposeMethod);
            }

            return typeSyntax;
        }

        private static BaseTypeDeclarationSyntax MoveCompleteRegion(BaseTypeDeclarationSyntax typeSyntax, SyntaxNode disposeMethod)
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
                        var modifiedTypeSyntax = typeSyntax;

                        var triviaToRemove = GetTriviaToRemove(endRegionTrivia);

                        // add an additional trivia here because the "#endregion" should be followed by an empty line
                        var triviaToAdd = GetTriviaToAdd(triviaToRemove, SyntaxFactory.EndOfLine(string.Empty));

                        var leadingTriviaOfDisposeMethod = disposeMethod.GetLeadingTrivia();

                        if (typeSyntax.IsEquivalentTo(parent))
                        {
                            parent = null;

                            // seems like "#endregion" was at the very end of the type, so remove it
                            modifiedTypeSyntax = typeSyntax.Without(triviaToRemove);

                            // and add "#endregion" to the trailing of the dispose method
                            disposeMethod = modifiedTypeSyntax.GetAnnotatedNodes(DisposeAnnotationKind).First();

                            if (disposeMethod.PreviousSibling() is MemberDeclarationSyntax)
                            {
                                // let's see if we need the trivia
                                var triviaOfRegion = GetTriviaToRemove(regionTrivia.ParentTrivia);

                                if (triviaOfRegion[0].IsWhiteSpace())
                                {
                                    // seems that a line break was replaced by a whitespace for the "#region", so we have to fix it back to a line break
                                    leadingTriviaOfDisposeMethod = leadingTriviaOfDisposeMethod.Replace(triviaOfRegion[0], SyntaxFactory.CarriageReturnLineFeed);

                                    // also ignore the added empty line of the trivia to add as it seems to be kept in that case
                                    triviaToAdd = triviaToRemove;
                                }
                            }
                        }

                        return modifiedTypeSyntax.ReplaceNodes(
                                                           new[] { parent, disposeMethod }.Where(_ => _ != null),
                                                           (original, rewritten) =>
                                                                                   {
                                                                                       if (rewritten.IsEquivalentTo(parent))
                                                                                       {
                                                                                           return rewritten.Without(triviaToRemove);
                                                                                       }

                                                                                       if (rewritten.IsEquivalentTo(disposeMethod))
                                                                                       {
                                                                                           return rewritten.WithAdditionalTrailingTrivia(triviaToAdd)
                                                                                                           .WithLeadingTrivia(leadingTriviaOfDisposeMethod);
                                                                                       }

                                                                                       return original;
                                                                                   });
                    }
                }
            }

            // should never happen as we have only a single region
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

        private static SyntaxTrivia[] GetTriviaToRemove(SyntaxTrivia trivia)
        {
            var result = Enumerable.Empty<SyntaxTrivia>()
                                   .Concat(trivia.PreviousSiblings(2))
                                   .Append(trivia)
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