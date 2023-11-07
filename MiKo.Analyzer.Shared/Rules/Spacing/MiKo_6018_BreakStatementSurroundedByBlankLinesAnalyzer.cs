using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6018_BreakStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<BreakStatementSyntax>
    {
        public const string Id = "MiKo_6018";

        public MiKo_6018_BreakStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.BreakStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(BreakStatementSyntax node) => node.BreakKeyword;
    }
}