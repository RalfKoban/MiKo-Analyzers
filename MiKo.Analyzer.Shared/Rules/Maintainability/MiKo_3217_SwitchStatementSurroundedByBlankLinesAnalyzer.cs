using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3217_SwitchStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<SwitchStatementSyntax>
    {
        public const string Id = "MiKo_3217";

        public MiKo_3217_SwitchStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.SwitchStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(SwitchStatementSyntax node) => node.SwitchKeyword;
    }
}