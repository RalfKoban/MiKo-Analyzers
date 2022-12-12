using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6009_TryStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<TryStatementSyntax>
    {
        public const string Id = "MiKo_6009";

        public MiKo_6009_TryStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.TryStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(TryStatementSyntax node) => node.TryKeyword;
    }
}