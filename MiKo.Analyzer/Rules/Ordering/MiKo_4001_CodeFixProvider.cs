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

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax syntax, Diagnostic diagnostic)
        {
            var semanticModel = document.GetSemanticModelAsync().Result;

            var method = diagnostic.Location.GetEnclosing<IMethodSymbol>(semanticModel);
            var methodName = method.Name;

            var methods = method.ContainingType.GetMembers(methodName).OfType<IMethodSymbol>();

            var methodsOrderedByParameters = MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer.GetMethodsOrderedByParameters(methods, methodName);

            var methodNodes = methodsOrderedByParameters.Select(_ => _.GetSyntax()).ToList();
            var orientationNode = methodNodes.First();

            // insert nodes after smallest
            methodNodes.Remove(orientationNode);

            // create new nodes
            var replacements = CreateReplacements(methodNodes);

            // fix tree
            var modifiedType = syntax.RemoveNodesAndAdjustOpenCloseBraces(methodNodes);
            var node = modifiedType.ChildNodes().First(_ => _.IsEquivalentTo(orientationNode));

            return modifiedType.InsertNodesAfter(node, replacements);
        }

        private static IEnumerable<SyntaxNode> CreateReplacements(IEnumerable<SyntaxNode> methodNodes)
        {
            var replacements = methodNodes.ToList();

            for (var i = 0; i < replacements.Count; i++)
            {
                // remove empty lines that could kept left over
                if (replacements[i].HasLeadingTrivia)
                {
                    replacements[i] = replacements[i].WithoutLeadingTrivia().WithLeadingEmptyLine().WithLeadingEndOfLine();
                }
            }

            return replacements;
        }
    }
}