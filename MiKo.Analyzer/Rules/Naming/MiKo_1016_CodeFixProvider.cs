using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1016_CodeFixProvider)), Shared]
    public sealed class MiKo_1016_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1016_FactoryMethodsAnalyzer.Id;

        protected override string Title => Resources.MiKo_1016_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected override string GetNewName(ISymbol symbol) => MiKo_1016_FactoryMethodsAnalyzer.FindBetterName((IMethodSymbol)symbol);
    }
}