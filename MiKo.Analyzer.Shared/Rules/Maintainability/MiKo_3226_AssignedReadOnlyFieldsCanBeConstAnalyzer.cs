using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3226_AssignedReadOnlyFieldsCanBeConstAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3226";

        public MiKo_3226_AssignedReadOnlyFieldsCanBeConstAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);

        private static bool HasIssue(VariableDeclarationSyntax declaration)
        {
            foreach (var variable in declaration.Variables)
            {
                if (variable.Initializer?.Value is LiteralExpressionSyntax literal)
                {
                    if (literal.IsKind(SyntaxKind.StringLiteralExpression) && declaration.Type.IsString() is false)
                    {
                        // cannot make constant as the type does not match
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        private void AnalyzeField(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is FieldDeclarationSyntax field && field.IsReadOnly())
            {
                if (HasIssue(field.Declaration))
                {
                    ReportDiagnostics(context, Issue(field));
                }
            }
        }
    }
}