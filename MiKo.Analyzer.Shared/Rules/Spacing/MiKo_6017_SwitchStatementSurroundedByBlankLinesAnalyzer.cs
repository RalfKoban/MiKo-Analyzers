using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6017_SwitchStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<SwitchStatementSyntax>
    {
        public const string Id = "MiKo_6017";

        public MiKo_6017_SwitchStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.SwitchStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(SwitchStatementSyntax node) => node.SwitchKeyword;
    }
}