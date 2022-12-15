using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class StatementSurroundedByBlankLinesAnalyzer<T> : SurroundedByBlankLinesAnalyzer where T : StatementSyntax
    {
        private readonly SyntaxKind m_syntaxKind;

        protected StatementSurroundedByBlankLinesAnalyzer(SyntaxKind syntaxKind, string id) : base(id) => m_syntaxKind = syntaxKind;

        protected abstract SyntaxToken GetKeyword(T node);

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeStatement, m_syntaxKind);
        }

        private void AnalyzeStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (T)context.Node;
            var issue = AnalyzeStatement(node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeStatement(T node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case BlockSyntax block:
                        return AnalyzeStatement(block, node);

                    case SwitchSectionSyntax section:
                        return AnalyzeStatement(section, node);

                    case MethodDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                        return null; // nothing more to look up as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeStatement(BlockSyntax block, T node)
        {
            var nodeLineSpan = node.GetLocation().GetLineSpan();

            var otherStatements = block.Statements.Except(node);

            var noBlankLinesBefore = otherStatements.Any(_ => HasNoBlankLinesBefore(nodeLineSpan, _));
            var noBlankLinesAfter = otherStatements.Any(_ => HasNoBlankLinesAfter(nodeLineSpan, _));

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(GetKeyword(node), noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }

        private Diagnostic AnalyzeStatement(SwitchSectionSyntax section, T node)
        {
            var nodeLineSpan = node.GetLocation().GetLineSpan();

            var statements = section.Statements;

            var noBlankLinesBefore = statements.Except(node).Any(_ => HasNoBlankLinesBefore(nodeLineSpan, _));
            var noBlankLinesAfter = statements.Except(node).Any(_ => HasNoBlankLinesAfter(nodeLineSpan, _));

            if (noBlankLinesAfter is false)
            {
                // inspect the switch section to see if another switch section directly comes after our node, as in such case we also need some blank lines
                var nextSection = section.NextSibling();

                if (nextSection != null && statements.Last() == node)
                {
                    // determine whether the next section has no blank line between itself and our node
                    noBlankLinesAfter = HasNoBlankLinesAfter(nodeLineSpan, nextSection);
                }
            }

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(GetKeyword(node), noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}