using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
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
                        return AnalyzeStatement(block.Statements, node);

                    case SwitchSectionSyntax section:
                        return AnalyzeStatement(section.Statements, node);

                    case MethodDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                        return null; // nothing more to look up as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeStatement(SyntaxList<StatementSyntax> statements, T node)
        {
            var callLineSpan = node.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements.Any(_ => HasNoBlankLinesBefore(callLineSpan, _));
            var noBlankLinesAfter = statements.Any(_ => HasNoBlankLinesAfter(callLineSpan, _));

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(GetKeyword(node), noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}