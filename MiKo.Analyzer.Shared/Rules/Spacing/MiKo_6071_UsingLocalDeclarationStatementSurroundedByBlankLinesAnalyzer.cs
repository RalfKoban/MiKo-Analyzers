using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6071_UsingLocalDeclarationStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<LocalDeclarationStatementSyntax>
    {
        public const string Id = "MiKo_6071";

        public MiKo_6071_UsingLocalDeclarationStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.LocalDeclarationStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(LocalDeclarationStatementSyntax node) => node.UsingKeyword;

        protected override bool ShallAnalyzeStatement(LocalDeclarationStatementSyntax node) => node.UsingKeyword.IsDefaultValue() is false;

        protected override bool ShallAnalyzeOtherStatement(StatementSyntax node)
        {
            if (node is LocalDeclarationStatementSyntax local)
            {
                // ignore other using statements
                return ShallAnalyzeStatement(local) is false;
            }

            return true;
        }
    }
}