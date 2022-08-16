using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
    {
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        public sealed class MiKo_3209_TryStatementSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
        {
            public const string Id = "MiKo_3209";

            public MiKo_3209_TryStatementSurroundedByBlankLinesAnalyzer() : base(Id)
            {
            }

            protected override void InitializeCore(CompilationStartAnalysisContext context)
            {
                context.RegisterSyntaxNodeAction(AnalyzeTryStatement, SyntaxKind.TryStatement);
            }

            private void AnalyzeTryStatement(SyntaxNodeAnalysisContext context)
            {
                var node = (TryStatementSyntax)context.Node;
                var issue = AnalyzeTryStatement(node);

                ReportDiagnostics(context, issue);
            }

            private Diagnostic AnalyzeTryStatement(TryStatementSyntax node)
            {
                foreach (var ancestor in node.Ancestors())
                {
                    switch (ancestor)
                    {
                        case BlockSyntax block:
                            return AnalyzeTryStatement(block.Statements, node);

                        case SwitchSectionSyntax section:
                            return AnalyzeTryStatement(section.Statements, node);

                        case MethodDeclarationSyntax _:
                        case ClassDeclarationSyntax _:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }

                return null;
            }

            private Diagnostic AnalyzeTryStatement(SyntaxList<StatementSyntax> statements, TryStatementSyntax node)
            {
                var callLineSpan = node.GetLocation().GetLineSpan();

                var noBlankLinesBefore = statements.Any(_ => HasNoBlankLinesBefore(callLineSpan, _));
                var noBlankLinesAfter = statements.Any(_ => HasNoBlankLinesAfter(callLineSpan, _));

                if (noBlankLinesBefore || noBlankLinesAfter)
                {
                    return Issue(node.TryKeyword, noBlankLinesBefore, noBlankLinesAfter);
                }

                return null;
            }
        }
    }