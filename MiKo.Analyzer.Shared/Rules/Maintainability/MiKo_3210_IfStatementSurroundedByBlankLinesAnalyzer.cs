using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3210_IfStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<IfStatementSyntax>
    {
        public const string Id = "MiKo_3210";

        public MiKo_3210_IfStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.IfStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(IfStatementSyntax node) => node.IfKeyword;
    }
}