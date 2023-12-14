using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class OrderingCodeFixProvider : MiKoCodeFixProvider
    {
        protected static BaseTypeDeclarationSyntax MoveCompleteRegion(BaseTypeDeclarationSyntax typeSyntax, SyntaxNode method, string annotationKind)
        {
            if (method.TryGetRegionDirective(out var regionDirective))
            {
                var relatedDirectives = regionDirective.GetRelatedDirectives();

                if (relatedDirectives.Count == 2)
                {
                    var regionTrivia = regionDirective.ParentTrivia;

                    var endRegionTrivia = relatedDirectives[1].ParentTrivia;
                    var parent = endRegionTrivia.Token.Parent;

                    if (parent != null)
                    {
                        var modifiedTypeSyntax = typeSyntax;

                        var triviaToRemove = GetTriviaToRemove(endRegionTrivia);

                        // add an additional trivia here because the "#endregion" should be followed by an empty line
                        var triviaToAdd = GetTriviaToAdd(triviaToRemove, SyntaxFactory.EndOfLine(string.Empty));

                        var leadingTriviaOfMethod = method.GetLeadingTrivia().ToList();
                        var startPosition = leadingTriviaOfMethod.IndexOf(regionTrivia);
                        leadingTriviaOfMethod = leadingTriviaOfMethod.Skip(startPosition - 2).ToList();

                        if (typeSyntax.IsEquivalentTo(parent))
                        {
                            // seems like "#endregion" was at the very end of the type, so remove it
                            modifiedTypeSyntax = typeSyntax.Without(triviaToRemove);

                            // and add "#endregion" to the trailing of the method
                            method = modifiedTypeSyntax.GetAnnotatedNodes(annotationKind).First();

                            if (method.PreviousSibling() is MemberDeclarationSyntax)
                            {
                                // let's see if we need the trivia
                                var triviaOfRegion = GetTriviaToRemove(regionTrivia);

                                if (triviaOfRegion[0].IsWhiteSpace())
                                {
                                    // seems that a line break was replaced by a whitespace for the "#region", so we have to fix it back to a line break
                                    var index = leadingTriviaOfMethod.FindIndex(_ => _.IsEquivalentTo(triviaOfRegion[0]));
                                    if (index >= 0)
                                    {
                                        leadingTriviaOfMethod[index] = SyntaxFactory.CarriageReturnLineFeed;
                                    }

                                    // also ignore the added empty line of the trivia to add as it seems to be kept in that case
                                    triviaToAdd = triviaToRemove;
                                }
                            }
                            else
                            {
                                parent = null;
                            }
                        }

                        return modifiedTypeSyntax.ReplaceNodes(
                                                           new[] { parent, method }.Where(_ => _ != null),
                                                           (original, rewritten) =>
                                                           {
                                                               if (rewritten.IsEquivalentTo(parent))
                                                               {
                                                                   return rewritten.Without(triviaToRemove);
                                                               }

                                                               if (rewritten.IsEquivalentTo(method))
                                                               {
                                                                   return rewritten.WithAdditionalTrailingTrivia(triviaToAdd)
                                                                                   .WithLeadingTrivia(leadingTriviaOfMethod);
                                                               }

                                                               return original;
                                                           });
                    }
                }
            }

            // should never happen as we have only a single region
            return typeSyntax;
        }

        protected static SyntaxTrivia[] GetTriviaToRemove(SyntaxTrivia trivia)
        {
            var result = Enumerable.Empty<SyntaxTrivia>()
                                   .Concat(trivia.PreviousSiblings(2))
                                   .Concat(new[] { trivia })
                                   .ToArray();

            return result;
        }

        protected static SyntaxTrivia[] GetTriviaToAdd(SyntaxTrivia[] triviaToRemove, SyntaxTrivia additionalTrivia)
        {
            var result = new SyntaxTrivia[triviaToRemove.Length + 1];
            result[triviaToRemove.Length] = additionalTrivia;

            triviaToRemove.CopyTo(result, 0);

            return result;
        }

        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            var typeSyntax = syntax.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();

            var updatedTypeSyntax = GetUpdatedTypeSyntax(document, typeSyntax, syntax, issue);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected abstract SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic);
    }
}