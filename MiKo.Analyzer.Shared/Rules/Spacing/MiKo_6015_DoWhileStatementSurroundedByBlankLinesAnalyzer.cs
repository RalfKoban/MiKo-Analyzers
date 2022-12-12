using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6015_DoWhileStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<DoStatementSyntax>
    {
        public const string Id = "MiKo_6015";

        public MiKo_6015_DoWhileStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.DoStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(DoStatementSyntax node) => node.DoKeyword;
    }
}