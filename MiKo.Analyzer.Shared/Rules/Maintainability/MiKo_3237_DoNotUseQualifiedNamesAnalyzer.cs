using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3237_DoNotUseQualifiedNamesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3237";

        public MiKo_3237_DoNotUseQualifiedNamesAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeQualifiedName, SyntaxKind.SimpleMemberAccessExpression);

        private static bool HasIssue(MemberAccessExpressionSyntax node, in SyntaxNodeAnalysisContext context)
        {
            if (node.Name is IdentifierNameSyntax identifier)
            {
                switch (node.Parent)
                {
                    case ArgumentSyntax argument when argument.Expression == node: // only a complete namespace as argument, so do not report it
                    case AssignmentExpressionSyntax _:
                    case BinaryExpressionSyntax _:
                    case ConditionalExpressionSyntax _:
                    case InvocationExpressionSyntax _: // we have an invocation, so this is no namespace
                    case IsPatternExpressionSyntax _:
                    case ParenthesizedExpressionSyntax _:
                    case PrefixUnaryExpressionSyntax _:
                    case PostfixUnaryExpressionSyntax _:
                        return false;
                }

                var symbol = identifier.GetSymbol(context.SemanticModel);

                if (symbol is INamespaceSymbol)
                {
                    if (node.Parent is MemberAccessExpressionSyntax parent && parent.Name is IdentifierNameSyntax identifierFromParent)
                    {
                        if (identifierFromParent.GetSymbol(context.SemanticModel) is INamespaceSymbol)
                        {
                            // ignore this as the parent is already reported
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private void AnalyzeQualifiedName(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MemberAccessExpressionSyntax node && HasIssue(node, context))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}