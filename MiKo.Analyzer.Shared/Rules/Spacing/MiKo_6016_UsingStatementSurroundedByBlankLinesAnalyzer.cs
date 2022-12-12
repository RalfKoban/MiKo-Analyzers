using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6016_UsingStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<UsingStatementSyntax>
    {
        public const string Id = "MiKo_6016";

        public MiKo_6016_UsingStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.UsingStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(UsingStatementSyntax node) => node.UsingKeyword;
    }
}