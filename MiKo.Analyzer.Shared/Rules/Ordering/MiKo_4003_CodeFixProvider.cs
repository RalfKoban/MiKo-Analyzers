using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4003_CodeFixProvider)), Shared]
    public sealed class MiKo_4003_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer.Id;

        protected override string Title => Resources.MiKo_4003_CodeFixTitle;

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var disposeMethod = (MethodDeclarationSyntax)syntax;

            var annotation = new SyntaxAnnotation("dispose method");

            // remove method so that it can be added again
            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(disposeMethod);

            var syntaxNode = FindLastCtorOrFinalizer(modifiedType);

            if (syntaxNode is null)
            {
                // none found, so insert method before first method
                var method = modifiedType.FirstChild<MethodDeclarationSyntax>();

                return MoveRegions(modifiedType.InsertNodeBefore(method, disposeMethod.WithAnnotation(annotation)), disposeMethod, annotation);
            }

            // insert method after found ctor or finalizer
            return MoveRegions(modifiedType.InsertNodeAfter(syntaxNode, disposeMethod.WithAnnotation(annotation)), disposeMethod, annotation);
        }

        private static BaseTypeDeclarationSyntax MoveRegions(BaseTypeDeclarationSyntax typeSyntax, SyntaxNode originalDisposeMethod, SyntaxAnnotation annotation)
        {
            if (IsSingleNodeInsideRegion(originalDisposeMethod) is false)
            {
                // seems like other methods or stuff is inside the "#region", so we do not move the "#endregion" around
                return typeSyntax;
            }

            var disposeMethod = typeSyntax.GetAnnotatedNodes(annotation).First();

            if (disposeMethod.TryGetLeadingRegion(out var regionTrivia))
            {
                var relatedDirectives = regionTrivia.GetRelatedDirectives();

                if (relatedDirectives.Count == 2)
                {
                    var endRegionTrivia = relatedDirectives[1].ParentTrivia;
                    var parent = endRegionTrivia.Token.Parent;

                    if (parent != null)
                    {
                        var triviaToRemove = GetTriviaToRemove(endRegionTrivia);
                        var triviaToAdd = GetTriviaToAdd(triviaToRemove);

                        if (typeSyntax.IsEquivalentTo(parent))
                        {
                            // seems like "#endregion" was at the very end of the type, so remove it
                            var syntax = typeSyntax.Without(triviaToRemove);

                            // and add "#endregion" to the trailing of the dispose method
                            disposeMethod = syntax.GetAnnotatedNodes(annotation).First();

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

                                                                               return null;
                                                                           });
                    }
                }
            }

            return typeSyntax;
        }

        private static bool IsSingleNodeInsideRegion(SyntaxNode node)
        {
            if (node.TryGetLeadingRegion(out var regionTrivia))
            {
                var relatedDirectives = regionTrivia.GetRelatedDirectives();

                if (relatedDirectives.Count == 2)
                {
                    var endRegionTrivia = relatedDirectives[1];

                    var otherSyntaxNode = endRegionTrivia.ParentTrivia.Token.Parent;

                    if (otherSyntaxNode != null)
                    {
                        if (otherSyntaxNode.IsEquivalentTo(node.NextSibling()))
                        {
                            return true;
                        }

                        if (otherSyntaxNode.IsEquivalentTo(node.Parent))
                        {
                            // seems like same type
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static SyntaxNode FindLastCtorOrFinalizer(SyntaxNode modifiedType)
        {
            SyntaxNode finalizer = modifiedType.LastChild<DestructorDeclarationSyntax>();

            return finalizer ?? modifiedType.LastChild<ConstructorDeclarationSyntax>();
        }

        private static SyntaxTrivia[] GetTriviaToRemove(SyntaxTrivia trivia)
        {
            var result = Enumerable.Empty<SyntaxTrivia>()
                                   .Concat(trivia.PreviousSiblings(2))
                                   .Concat(new[] { trivia })
                                   .ToArray();

            return result;
        }

        private static SyntaxTrivia[] GetTriviaToAdd(SyntaxTrivia[] trivia)
        {
            var result = new SyntaxTrivia[trivia.Length + 1];
            trivia.CopyTo(result, 0);

            // add an additional empty line here because the "#endregion" should be followed by an empty line
            result[trivia.Length] = SyntaxFactory.EndOfLine(string.Empty);

            return result;
        }
    }
}