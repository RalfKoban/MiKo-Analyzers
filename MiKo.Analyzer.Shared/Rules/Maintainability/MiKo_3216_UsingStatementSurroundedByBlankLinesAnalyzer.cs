using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3216_UsingStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<UsingStatementSyntax>
    {
        public const string Id = "MiKo_3216";

        public MiKo_3216_UsingStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.UsingStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(UsingStatementSyntax node) => node.UsingKeyword;
    }
}