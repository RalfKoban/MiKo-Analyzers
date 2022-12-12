using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6011_LockStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<LockStatementSyntax>
    {
        public const string Id = "MiKo_6011";

        public MiKo_6011_LockStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.LockStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(LockStatementSyntax node) => node.LockKeyword;
    }
}