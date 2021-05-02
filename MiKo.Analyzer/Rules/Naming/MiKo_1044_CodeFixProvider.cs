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

        protected override string Title => "Append '" + MiKo_1044_CommandSuffixAnalyzer.Suffix + "' suffix";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => symbol.Name + MiKo_1044_CommandSuffixAnalyzer.Suffix;
    }
}