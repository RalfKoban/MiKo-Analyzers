using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6014_WhileStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<WhileStatementSyntax>
    {
        public const string Id = "MiKo_6014";

        public MiKo_6014_WhileStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.WhileStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(WhileStatementSyntax node) => node.WhileKeyword;
    }
}