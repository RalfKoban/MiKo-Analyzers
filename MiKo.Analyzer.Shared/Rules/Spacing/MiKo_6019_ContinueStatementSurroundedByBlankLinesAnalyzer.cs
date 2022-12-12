using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6019_ContinueStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<ContinueStatementSyntax>
    {
        public const string Id = "MiKo_6019";

        public MiKo_6019_ContinueStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.ContinueStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(ContinueStatementSyntax node) => node.ContinueKeyword;
    }
}