using System;
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
        private const string DisposeAnnotationKind = "dispose method";

        public override string FixableDiagnosticId => MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer.Id;

        protected override string Title => Resources.MiKo_4003_CodeFixTitle;

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var disposeMethod = (MethodDeclarationSyntax)syntax;

            var annotation = new SyntaxAnnotation(DisposeAnnotationKind);

            // remove method so that it can be added again
            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(disposeMethod);

            var syntaxNode = FindLastCtorOrFinalizer(modifiedType);

            if (syntaxNode is null)
            {
                // none found, so insert method before first method
                var method = modifiedType.FirstChild<MethodDeclarationSyntax>();

                modifiedType = MoveRegions(modifiedType.InsertNodeBefore(method, disposeMethod.WithAnnotation(annotation)), disposeMethod);
            }
            else
            {
                // insert method after found ctor or finalizer
                modifiedType = MoveRegions(modifiedType.InsertNodeAfter(syntaxNode, disposeMethod.WithAnnotation(annotation)), disposeMethod);
            }

            return modifiedType.WithoutAnnotations(annotation);
        }

        private static BaseTypeDeclarationSyntax MoveRegions(BaseTypeDeclarationSyntax typeSyntax, SyntaxNode originalDisposeMethod)
        {
            var disposeMethod = typeSyntax.GetAnnotatedNodes(DisposeAnnotationKind).First();

            if (IsSingleNodeInsideRegion(originalDisposeMethod))
            {
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

                                                                                   return null;
                                                                               });
                        }
                    }
                }
            }

            // seems like other methods or stuff are inside the "#region", so we do not move the "#endregion" around, but maybe the "#region"
            if (disposeMethod.NextSibling() is MethodDeclarationSyntax method)
            {
                if (method.TryGetLeadingRegion(out var regionDirective))
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

                                                                           return null;
                                                                       });
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

        private static SyntaxTrivia[] GetTriviaToAdd(SyntaxTrivia[] triviaToRemove, SyntaxTrivia additionalTrivia)
        {
            var result = new SyntaxTrivia[triviaToRemove.Length + 1];
            result[triviaToRemove.Length] = additionalTrivia;

            triviaToRemove.CopyTo(result, 0);

            return result;
        }
    }
}