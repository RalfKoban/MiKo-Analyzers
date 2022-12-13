using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6020_BreakStatementSurroundedByBlankLinesAnalyzer : StatementSurroundedByBlankLinesAnalyzer<BreakStatementSyntax>
    {
        public const string Id = "MiKo_6020";

        public MiKo_6020_BreakStatementSurroundedByBlankLinesAnalyzer() : base(SyntaxKind.BreakStatement, Id)
        {
        }

        protected override SyntaxToken GetKeyword(BreakStatementSyntax node) => node.BreakKeyword;
        //
        // protected override bool SpecialCareNoBlankLinesAfter(SyntaxList<StatementSyntax> statements, BreakStatementSyntax node, out bool result)
        // {
        //     if (node.Parent is SwitchSectionSyntax section && section.Parent is SwitchStatementSyntax switchStatement)
        //     {
        //         var sections = switchStatement.Sections;
        //         var index = sections.IndexOf(section);
        //
        //         var isNotLastSection = index < sections.Count - 1;
        //         if (isNotLastSection)
        //         {
        //             var isLastNodeInsideSection = statements.Last() == node;
        //             if (isLastNodeInsideSection)
        //             {
        //                 // determine whether the next section has no blank line between itself and our node
        //                 var nextSection = sections[index + 1];
        //
        //                 if (HasNoBlankLinesBefore(nextSection, node))
        //                 {
        //                     result = true;
        //
        //                     return true;
        //                 }
        //             }
        //         }
        //     }
        //
        //     result = false;
        //
        //     return false;
        // }
    }
}