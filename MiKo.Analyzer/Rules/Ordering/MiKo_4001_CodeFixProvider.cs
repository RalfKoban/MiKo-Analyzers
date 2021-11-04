using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
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
            var semanticModel = document.GetSemanticModelAsync().Result;

            var method = diagnostic.Location.GetEnclosing<IMethodSymbol>(semanticModel);
            var methodName = method.Name;

            var methods = method.ContainingType.GetMembers(methodName).OfType<IMethodSymbol>();

            var methodsOrderedByParameters = MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer.GetMethodsOrderedByParameters(methods, methodName);

            var modifiedType = typeSyntax;

            // handle each accessibility separately, to avoid situations such as that private methods suddenly get sorted before other public methods
            foreach (var nodes in methodsOrderedByParameters.GroupBy(_ => _.DeclaredAccessibility))
            {
                var methodNodes = nodes.Select(_ => _.GetSyntax()).ToList();
                if (methodNodes.Count == 1)
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

                var node = modifiedType.ChildNodes().First(_ => _.IsEquivalentTo(orientationNode));

                modifiedType = modifiedType.InsertNodesAfter(node, replacements);
            }

            return modifiedType;
        }

        private static IEnumerable<SyntaxNode> CreateReplacements(IEnumerable<SyntaxNode> methodNodes)
        {
            var replacements = methodNodes.ToList();

            for (var i = 0; i < replacements.Count; i++)
            {
                // remove empty lines that could kept left over
                // if (replacements[i].HasLeadingTrivia)
                {
                    // Attention: leading trivia contains XML comments, so we have to keep them!
                    replacements[i] = replacements[i].WithLeadingEmptyLine().WithLeadingEndOfLine();
                }
            }

            return replacements;
        }
    }
}