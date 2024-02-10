using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6020_ThrowStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<ThrowStatementSyntax>
    {
        public const string Id = "MiKo_6020";

        public MiKo_6020_ThrowStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.ThrowStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(ThrowStatementSyntax node) => node.ThrowKeyword;
    }
}