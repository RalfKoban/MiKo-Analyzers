using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4001_CodeFixProvider)), Shared]
    public sealed class MiKo_4001_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer.Id;

        protected override string Title => Resources.MiKo_4001_CodeFixTitle;

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var method = diagnostic.Location.GetEnclosing<IMethodSymbol>(GetSemanticModel(document));
            var methodName = method.Name;

            var methods = method.ContainingType.GetMembers(methodName).OfType<IMethodSymbol>();

            var methodsOrderedByParameters = MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer.GetMethodsOrderedByParameters(methods, methodName);

            var modifiedType = typeSyntax;

            // handle each accessibility separately, to avoid situations such as that private methods suddenly get sorted before other public methods
            foreach (var nodes in methodsOrderedByParameters.GroupBy(_ => _.DeclaredAccessibility))
            {
                var methodNodes = nodes.Select(_ => _.GetSyntax()).Where(_ => _ != null).ToList();

                if (methodNodes.Count <= 1)
                {
                    // ignore those that have only one occurrence
                    continue;
                }

                // recreate nodes of type as we might change the type multiple times and would otherwise have the references to the outdated, previously changed, type
                var childNodes = modifiedType.ChildNodes().ToList();
                methodNodes = methodNodes.Select(_ => childNodes.First(_.IsEquivalentTo)).ToList();

                var orientationNode = methodNodes.First();

                // insert nodes after smallest
                methodNodes.Remove(orientationNode);

                // create new nodes
                var replacements = CreateReplacements(methodNodes);

                // fix tree
                modifiedType = modifiedType.RemoveNodesAndAdjustOpenCloseBraces(methodNodes);

                var node = modifiedType.FirstChild(_ => _.IsEquivalentTo(orientationNode));

                modifiedType = modifiedType.InsertNodesAfter(node, replacements);

                // adjust #region directive
                modifiedType = AdjustRegion(modifiedType, replacements, typeSyntax, methodNodes);
            }

            return modifiedType;
        }

        private static IEnumerable<SyntaxNode> CreateReplacements(IEnumerable<SyntaxNode> methodNodes)
        {
            var replacements = methodNodes.ToList();

            for (var i = 0; i < replacements.Count; i++)
            {
                // Attention: leading trivia contains XML comments, so we have to keep them!
                replacements[i] = replacements[i].WithLeadingEmptyLine().WithLeadingEndOfLine();
            }

            return replacements;
        }

        private static BaseTypeDeclarationSyntax AdjustRegion(BaseTypeDeclarationSyntax modifiedType, IEnumerable<SyntaxNode> replacedMethods, SyntaxNode originalType, IEnumerable<SyntaxNode> originalMethods)
        {
            var methodWithRegion = replacedMethods.FirstOrDefault(_ => _.HasRegionDirective());

            if (methodWithRegion.TryGetRegionDirective(out var region))
            {
                var regionTrivia = region.ParentTrivia;

                // determine indices in original code, to apply regions properly
                var originalMethodWithRegion = originalMethods.FirstOrDefault(_ => _.HasRegionDirective());
                var originalRegionIndex = originalType.ChildNodes().IndexOf(_ => _.IsEquivalentTo(originalMethodWithRegion));

                var oldRegionStartNode = modifiedType.FirstChild(_ => _.IsEquivalentTo(methodWithRegion));
                var newRegionStartNode = modifiedType.ChildNodes().ElementAt(originalRegionIndex);

                modifiedType = modifiedType.ReplaceNodes(
                                                         new[] { oldRegionStartNode, newRegionStartNode },
                                                         (original, rewritten) =>
                                                         {
                                                             if (rewritten.IsEquivalentTo(oldRegionStartNode))
                                                             {
                                                                 if (rewritten.TryGetRegionDirective(out var regionDirective))
                                                                 {
                                                                     var trivia = regionDirective.ParentTrivia.Token.LeadingTrivia;
                                                                     var index = trivia.IndexOf(SyntaxKind.RegionDirectiveTrivia);

                                                                     if (index > 0)
                                                                     {
                                                                         // some comments available, so do not delete them (but keep end-of-line)
                                                                         return rewritten.WithLeadingTrivia(trivia.Skip(index + 1));
                                                                     }

                                                                     return rewritten.RemoveTrivia(regionDirective.ParentTrivia);
                                                                 }
                                                             }
                                                             else if (rewritten.IsEquivalentTo(newRegionStartNode))
                                                             {
                                                                 var trivia = regionTrivia.Token.LeadingTrivia;
                                                                 var index = trivia.IndexOf(SyntaxKind.SingleLineDocumentationCommentTrivia);

                                                                 if (index > 0)
                                                                 {
                                                                     // some comments available, so do not copy them (and also remove end-of-line)
                                                                     rewritten = rewritten.WithFirstLeadingTrivia(trivia.Take(index - 1).ToArray());
                                                                 }
                                                                 else
                                                                 {
                                                                     rewritten = rewritten.WithLeadingTrivia(trivia);
                                                                 }

                                                                 if (original.PreviousSibling() != null)
                                                                 {
                                                                     // add an empty line only in case we did not move the method to be the very first
                                                                     return rewritten.WithLeadingEmptyLine();
                                                                 }
                                                             }

                                                             return rewritten;
                                                         });
            }

            return modifiedType;
        }
    }
}