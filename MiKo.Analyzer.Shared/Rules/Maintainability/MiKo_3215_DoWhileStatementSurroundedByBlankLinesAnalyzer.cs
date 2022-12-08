using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3215_DoWhileStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<DoStatementSyntax>
    {
        public const string Id = "MiKo_3215";

        public MiKo_3215_DoWhileStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.DoStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(DoStatementSyntax node) => node.DoKeyword;
    }
}