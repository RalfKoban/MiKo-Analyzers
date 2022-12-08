using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3214_WhileStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<WhileStatementSyntax>
    {
        public const string Id = "MiKo_3214";

        public MiKo_3214_WhileStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.WhileStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(WhileStatementSyntax node) => node.WhileKeyword;
    }
}