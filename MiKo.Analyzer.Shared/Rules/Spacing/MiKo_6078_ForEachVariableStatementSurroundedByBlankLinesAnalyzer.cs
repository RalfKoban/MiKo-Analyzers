using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6078_ForEachVariableStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<ForEachVariableStatementSyntax>
    {
        public const string Id = "MiKo_6078";

        public MiKo_6078_ForEachVariableStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.ForEachVariableStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(ForEachVariableStatementSyntax node) => node.ForEachKeyword;
    }
}