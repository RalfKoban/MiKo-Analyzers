using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4002_CodeFixProvider)), Shared]
    public sealed class MiKo_4002_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer.Id;

        protected override string Title => Resources.MiKo_4002_CodeFixTitle;

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var method = diagnostic.Location.GetEnclosing<IMethodSymbol>(GetSemanticModel(document));
            var methodName = method.Name;

            var methods = method.ContainingType.GetMembers(methodName).OfType<IMethodSymbol>();

            var methodsOrderedByParameters = MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer.GetMethodsOrderedByStatics(methods, methodName);

            var methodNodes = methodsOrderedByParameters.Select(_ => _.GetSyntax()).ToList();
            var orientationNode = methodNodes.First();

            // insert nodes after smallest
            methodNodes.Remove(orientationNode);

            // create new nodes
            var replacements = CreateReplacements(methodNodes);

            // fix tree
            var modifiedType = typeSyntax.RemoveNodesAndAdjustOpenCloseBraces(methodNodes);
            var node = modifiedType.FirstChild(_ => _.IsEquivalentTo(orientationNode));

            return modifiedType.InsertNodesAfter(node, replacements);
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
    }
}