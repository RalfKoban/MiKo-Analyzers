using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1061_CodeFixProvider)), Shared]
    public sealed class MiKo_1061_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1061_TryMethodOutParameterNameAnalyzer.Id;

        protected override string Title => "Rename out parameter";

        protected override string GetNewName(ISymbol symbol) => MiKo_1061_TryMethodOutParameterNameAnalyzer.FindBetterName(symbol.GetEnclosingMethod());

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().FirstOrDefault();
    }
}