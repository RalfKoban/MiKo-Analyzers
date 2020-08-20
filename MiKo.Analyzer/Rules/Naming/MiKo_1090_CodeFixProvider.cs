using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1090_CodeFixProvider)), Shared]
    public sealed class MiKo_1090_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1090_ParametersWrongSuffixedAnalyzer.Id;

        protected override string Title => "Rename parameter suffix";

        protected override string GetNewName(ISymbol symbol) => MiKo_1090_ParametersWrongSuffixedAnalyzer.FindBetterName(symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().First();
    }
}