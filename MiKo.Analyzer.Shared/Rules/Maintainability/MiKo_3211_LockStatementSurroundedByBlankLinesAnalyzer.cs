using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3211_LockStatementSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3211";

        public MiKo_3211_LockStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LockStatementSyntax)context.Node;
            var issue = AnalyzeLockStatement(node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeLockStatement(LockStatementSyntax node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case BlockSyntax block:
                        return AnalyzeLockStatement(block.Statements, node);

                    case SwitchSectionSyntax section:
                        return AnalyzeLockStatement(section.Statements, node);

                    case MethodDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                        return null; // stop lookup as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeLockStatement(SyntaxList<StatementSyntax> statements, LockStatementSyntax node)
        {
            var callLineSpan = node.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements.Any(_ => HasNoBlankLinesBefore(callLineSpan, _));
            var noBlankLinesAfter = statements.Any(_ => HasNoBlankLinesAfter(callLineSpan, _));

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(node.LockKeyword, noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}