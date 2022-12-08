using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3209_TryStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<TryStatementSyntax>
    {
        public const string Id = "MiKo_3209";

        public MiKo_3209_TryStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.TryStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(TryStatementSyntax node) => node.TryKeyword;
    }
}