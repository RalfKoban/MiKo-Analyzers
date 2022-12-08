using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3212_ForEachStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<ForEachStatementSyntax>
    {
        public const string Id = "MiKo_3212";

        public MiKo_3212_ForEachStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.ForEachStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(ForEachStatementSyntax node) => node.ForEachKeyword;
    }
}