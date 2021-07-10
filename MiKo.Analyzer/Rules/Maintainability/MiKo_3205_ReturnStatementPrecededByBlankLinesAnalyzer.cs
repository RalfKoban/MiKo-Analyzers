using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3205_ReturnStatementPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3205";

        public MiKo_3205_ReturnStatementPrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeReturnStatementSyntax, SyntaxKind.ReturnStatement);
        }

        private void AnalyzeReturnStatementSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (ReturnStatementSyntax)context.Node;

            var diagnostic = AnalyzeReturnStatementSyntax(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeReturnStatementSyntax(ReturnStatementSyntax statement, SemanticModel semanticModel)
        {
            var callLineSpan = statement.GetLocation().GetLineSpan();

            foreach (var ancestor in statement.Ancestors())
            {
                // if (ancestor is ParenthesizedLambdaExpressionSyntax)
                // {
                //     // no issue
                //     break;
                // }
                if (ancestor is BlockSyntax block)
                {
                    var noBlankLinesBefore = block.Statements
                                                  .Any(_ => HasNoBlankLinesBefore(callLineSpan, _));

                    if (noBlankLinesBefore)
                    {
                        return Issue(statement, true, false);
                    }

                    break;
                }

                if (ancestor is MethodDeclarationSyntax || ancestor is ClassDeclarationSyntax)
                {
                    // stop lookup as there is no valid ancestor anymore
                    break;
                }
            }

            return null;
        }
    }
}