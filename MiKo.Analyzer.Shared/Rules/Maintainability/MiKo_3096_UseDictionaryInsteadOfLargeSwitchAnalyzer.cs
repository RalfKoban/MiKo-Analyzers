using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3096";

        private const int MinimumCases = 7;

        public MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement, SyntaxKind.SwitchExpression);

        private static bool IsAcceptable(SyntaxNodeAnalysisContext context, StatementSyntax syntax)
        {
            switch (syntax)
            {
                case ReturnStatementSyntax statement:
                    return IsAcceptable(context, statement.Expression);

                case ThrowStatementSyntax _:
                    return true;

                case BlockSyntax block:
                {
                    var statements = block.Statements;

                    return statements.Count == 1 && IsAcceptable(context, statements[0]);
                }

                default:
                    return false;
            }
        }

        private static bool IsAcceptable(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax)
        {
            switch (syntax)
            {
                case MemberAccessExpressionSyntax member:
                    return IsAcceptable(context, member);

                case LiteralExpressionSyntax _:
                    return true;

                case PrefixUnaryExpressionSyntax prefixed:
                    return prefixed.Operand is LiteralExpressionSyntax;

                case ThrowExpressionSyntax _:
                    return true;

                case TypeOfExpressionSyntax _:
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsAcceptable(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax member)
        {
            if (member.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                switch (member.Expression)
                {
                    case PredefinedTypeSyntax _:
                        return true;

                    case IdentifierNameSyntax identifier:
                        return identifier.GetTypeSymbol(context.SemanticModel).IsEnum();
                }
            }

            return false;
        }

        private static bool HasIssue(SyntaxNodeAnalysisContext context, SyntaxList<SwitchSectionSyntax> sections)
        {
            var sectionsCount = sections.Count;

            if (sectionsCount <= MinimumCases)
            {
                return false;
            }

            var throws = 0;

            for (var index = 0; index < sectionsCount; index++)
            {
                var statements = sections[index].Statements;

                if (statements.Count != 1)
                {
                    return false;
                }

                var statement = statements[0];

                if (IsAcceptable(context, statement) is false)
                {
                    return false;
                }

                if (statement.IsKind(SyntaxKind.ThrowStatement))
                {
                    throws++;
                }
            }

            if (throws > 1)
            {
                // cannot be converted
                return false;
            }

            return true;
        }

        private static bool HasIssue(SyntaxNodeAnalysisContext context, SeparatedSyntaxList<SwitchExpressionArmSyntax> arms)
        {
            var armsCount = arms.Count;

            if (armsCount <= MinimumCases)
            {
                return false;
            }

            var throws = 0;

            for (var index = 0; index < armsCount; index++)
            {
                var expression = arms[index].Expression;

                if (IsAcceptable(context, expression) is false)
                {
                    return false;
                }

                if (expression.IsKind(SyntaxKind.ThrowExpression))
                {
                    throws++;
                }
            }

            if (throws > 1)
            {
                // cannot be converted
                return false;
            }

            return true;
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case SwitchStatementSyntax statement:
                    AnalyzeSwitchStatement(context, statement);

                    break;

                case SwitchExpressionSyntax expression:
                    AnalyzeSwitchExpression(context, expression);

                    break;
            }
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context, SwitchStatementSyntax syntax)
        {
            if (HasIssue(context, syntax.Sections))
            {
                ReportDiagnostics(context, Issue(syntax));
            }
        }

        private void AnalyzeSwitchExpression(SyntaxNodeAnalysisContext context, SwitchExpressionSyntax syntax)
        {
            if (HasIssue(context, syntax.Arms))
            {
                ReportDiagnostics(context, Issue(syntax));
            }
        }
    }
}