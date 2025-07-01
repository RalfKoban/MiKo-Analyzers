using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class StatementSurroundedByBlankLinesAnalyzer<T> : SurroundedByBlankLinesAnalyzer where T : StatementSyntax
    {
        private readonly SyntaxKind[] m_syntaxKinds;

        protected StatementSurroundedByBlankLinesAnalyzer(in SyntaxKind syntaxKind, string id) : base(id) => m_syntaxKinds = new[] { syntaxKind };

        protected abstract SyntaxToken GetKeyword(T node);

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeStatement, m_syntaxKinds);

        protected virtual bool ShallAnalyzeStatement(T node) => true;

        protected virtual bool ShallAnalyzeOtherStatement(StatementSyntax node) => true;

        private static bool HasNoBlankLinesAfter(List<StatementSyntax> otherStatements, in FileLinePositionSpan afterPosition)
        {
            for (int index = 0, count = otherStatements.Count; index < count; index++)
            {
                if (HasNoBlankLinesAfter(afterPosition, otherStatements[index]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasNoBlankLinesBefore(List<StatementSyntax> otherStatements, in FileLinePositionSpan beforePosition)
        {
            for (int index = 0, count = otherStatements.Count; index < count; index++)
            {
                if (HasNoBlankLinesBefore(beforePosition, otherStatements[index]))
                {
                    return true;
                }
            }

            return false;
        }

        private void AnalyzeStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (T)context.Node;

            if (ShallAnalyzeStatement(node))
            {
                var issue = AnalyzeStatement(node);

                if (issue != null)
                {
                    ReportDiagnostics(context, issue);
                }
            }
        }

        private Diagnostic AnalyzeStatement(T node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.Block:
                        return AnalyzeStatement((BlockSyntax)ancestor, node);

                    case SyntaxKind.SwitchSection:
                        return AnalyzeStatement((SwitchSectionSyntax)ancestor, node);

                    // base methods
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                        return null; // stop lookup as there is no valid ancestor anymore

                    // base types
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.StructDeclaration:
                        return null; // stop lookup as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeStatement(BlockSyntax block, T node)
        {
            var otherStatements = block.Statements.Except(node).Where(ShallAnalyzeOtherStatement).ToList();

            if (otherStatements.Count > 0)
            {
                var beforePosition = GetLocationOfNodeOrLeadingComment(node).GetLineSpan();
                var afterPosition = GetLocationOfNodeOrTrailingComment(node).GetLineSpan();

                var noBlankLinesBefore = HasNoBlankLinesBefore(otherStatements, beforePosition);
                var noBlankLinesAfter = HasNoBlankLinesAfter(otherStatements, afterPosition);

                if (noBlankLinesBefore || noBlankLinesAfter)
                {
                    return Issue(GetKeyword(node), noBlankLinesBefore, noBlankLinesAfter);
                }
            }

            return null;
        }

        private Diagnostic AnalyzeStatement(SwitchSectionSyntax section, T node)
        {
            var beforePosition = GetLocationOfNodeOrLeadingComment(node).GetLineSpan();
            var afterPosition = GetLocationOfNodeOrTrailingComment(node).GetLineSpan();

            var statements = section.Statements;
            var otherStatements = statements.Except(node).Where(ShallAnalyzeOtherStatement).ToList();

            var noBlankLinesBefore = HasNoBlankLinesBefore(otherStatements, beforePosition);
            var noBlankLinesAfter = HasNoBlankLinesAfter(otherStatements, afterPosition);

            if (noBlankLinesAfter is false)
            {
                // inspect the switch section to see if another switch section directly comes after our node, as in such case we also need some blank lines
                var nextSection = section.NextSibling();

                if (nextSection != null && statements.Last() == node)
                {
                    // determine whether the next section has no blank line between itself and our node
                    noBlankLinesAfter = HasNoBlankLinesAfter(afterPosition, nextSection);
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