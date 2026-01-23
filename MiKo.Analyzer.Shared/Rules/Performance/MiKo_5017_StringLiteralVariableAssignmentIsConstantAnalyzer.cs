using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5017_StringLiteralVariableAssignmentIsConstantAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5017";

        public MiKo_5017_StringLiteralVariableAssignmentIsConstantAnalyzer() : base(Id)
        {
        }

        protected override DiagnosticSeverity Severity => DiagnosticSeverity.Info;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeStringLiteral, SyntaxKind.StringLiteralExpression);

        private void AnalyzeStringLiteral(SyntaxNodeAnalysisContext context)
        {
            var issue = AnalyzeStringLiteral(context.Node);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeStringLiteral(SyntaxNode node)
        {
            if (node is LiteralExpressionSyntax literal && literal.Parent is EqualsValueClauseSyntax e && e.Parent is VariableDeclaratorSyntax v && v.Parent is VariableDeclarationSyntax declaration)
            {
                switch (declaration.Parent)
                {
                    case LocalDeclarationStatementSyntax localVariable when localVariable.IsConst is false:
                    {
                        if (localVariable.Parent is BlockSyntax block)
                        {
                            var variableName = v.Identifier.ValueText;

                            if (block.DescendantNodes<ExpressionStatementSyntax>().Any(_ => _.Expression is AssignmentExpressionSyntax a && a.Left is IdentifierNameSyntax i && i.GetName() == variableName))
                            {
                                // the variable gets reassigned, so we do not report an issue
                                return null;
                            }
                        }

                        return Issue(localVariable);
                    }

                    case FieldDeclarationSyntax field when field.IsConst() is false && field.Declaration.Type.IsString():
                    {
                        return Issue(field);
                    }
                }
            }

            return null;
        }
    }
}