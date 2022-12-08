using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3213_ForStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<ForStatementSyntax>
    {
        public const string Id = "MiKo_3213";

        public MiKo_3213_ForStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.ForStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(ForStatementSyntax node) => node.ForKeyword;
    }
}