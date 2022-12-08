using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3211_LockStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<LockStatementSyntax>
    {
        public const string Id = "MiKo_3211";

        public MiKo_3211_LockStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.LockStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(LockStatementSyntax node) => node.LockKeyword;
    }
}