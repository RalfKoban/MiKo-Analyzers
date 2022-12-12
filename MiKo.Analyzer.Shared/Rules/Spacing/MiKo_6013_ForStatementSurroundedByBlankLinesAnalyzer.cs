using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6013_ForStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<ForStatementSyntax>
    {
        public const string Id = "MiKo_6013";

        public MiKo_6013_ForStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.ForStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(ForStatementSyntax node) => node.ForKeyword;
    }
}