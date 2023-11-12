using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1044_CodeFixProvider)), Shared]
    public sealed class MiKo_1044_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1044_CommandSuffixAnalyzer.Id;

        protected override string Title => Resources.MiKo_1044_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1044_CommandSuffixAnalyzer.FindBetterName(symbol, diagnostic);
    }
}