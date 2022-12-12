using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6010_IfStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<IfStatementSyntax>
    {
        public const string Id = "MiKo_6010";

        public MiKo_6010_IfStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.IfStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(IfStatementSyntax node) => node.IfKeyword;
    }
}