using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6082_LocalFunctionsSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<LocalFunctionStatementSyntax>
    {
        public const string Id = "MiKo_6082";

        public MiKo_6082_LocalFunctionsSurroundedByBlankLinesAnalyzer() : base(Id, SyntaxKind.LocalFunctionStatement)
        {
        }

        protected override SyntaxToken GetKeyword(LocalFunctionStatementSyntax node) => node.Identifier;
    }
}