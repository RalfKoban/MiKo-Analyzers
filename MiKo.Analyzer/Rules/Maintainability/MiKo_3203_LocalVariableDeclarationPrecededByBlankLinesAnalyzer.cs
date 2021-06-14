using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3203_LocalVariableDeclarationPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3203";

        public MiKo_3203_LocalVariableDeclarationPrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        private static bool IsDeclaration(StatementSyntax statement, SemanticModel semanticModel) => statement is LocalDeclarationStatementSyntax;

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;

            var diagnostic = AnalyzeLocalDeclarationStatement(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeLocalDeclarationStatement(StatementSyntax declaration, SemanticModel semanticModel)
        {
            if (IsDeclaration(declaration, semanticModel))
            {
                var block = declaration.Ancestors().OfType<BlockSyntax>().FirstOrDefault();
                if (block != null)
                {
                    var callLineSpan = declaration.GetLocation().GetLineSpan();

                    var noBlankLinesBefore = block.Statements
                                                  .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                                  .Any(_ => IsDeclaration(_, semanticModel) is false);

                    if (noBlankLinesBefore)
                    {
                        return Issue(declaration, true, false);
                    }
                }
            }

            return null;
        }
    }
}