using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3504_TrainWreckAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3504";

        private const int AllowedDepth = 4;

        private static readonly SyntaxKind[] AssignmentExpressionKinds =
                                                                         {
                                                                             SyntaxKind.AddAssignmentExpression,
                                                                             SyntaxKind.AndAssignmentExpression,
                                                                             SyntaxKind.CoalesceAssignmentExpression,
                                                                             SyntaxKind.DivideAssignmentExpression,
                                                                             SyntaxKind.ExclusiveOrAssignmentExpression,
                                                                             SyntaxKind.LeftShiftAssignmentExpression,
                                                                             SyntaxKind.ModuloAssignmentExpression,
                                                                             SyntaxKind.MultiplyAssignmentExpression,
                                                                             SyntaxKind.OrAssignmentExpression,
                                                                             SyntaxKind.RightShiftAssignmentExpression,
                                                                             SyntaxKind.SimpleAssignmentExpression,
                                                                             SyntaxKind.SubtractAssignmentExpression,
#if VS2022 || VS2026
                                                                             SyntaxKind.UnsignedRightShiftAssignmentExpression,
#endif
                                                                         };

        private static readonly SyntaxKind[] BinaryExpressionKinds =
                                                                     {
                                                                         SyntaxKind.AddExpression,
                                                                         SyntaxKind.AsExpression,
                                                                         SyntaxKind.BitwiseAndExpression,
                                                                         SyntaxKind.BitwiseOrExpression,
                                                                         SyntaxKind.CoalesceExpression,
                                                                         SyntaxKind.DivideExpression,
                                                                         SyntaxKind.EqualsExpression,
                                                                         SyntaxKind.ExclusiveOrExpression,
                                                                         SyntaxKind.GreaterThanExpression,
                                                                         SyntaxKind.GreaterThanOrEqualExpression,
                                                                         SyntaxKind.IsExpression,
                                                                         SyntaxKind.LeftShiftExpression,
                                                                         SyntaxKind.LessThanExpression,
                                                                         SyntaxKind.LessThanOrEqualExpression,
                                                                         SyntaxKind.LogicalAndExpression,
                                                                         SyntaxKind.LogicalOrExpression,
                                                                         SyntaxKind.ModuloExpression,
                                                                         SyntaxKind.MultiplyExpression,
                                                                         SyntaxKind.NotEqualsExpression,
                                                                         SyntaxKind.RightShiftExpression,
                                                                         SyntaxKind.SubtractExpression,
#if VS2022 || VS2026
                                                                         SyntaxKind.UnsignedRightShiftExpression,
#endif
                                                                     };

        private static readonly SyntaxKind[] PrefixUnaryExpressionKinds =
                                                                          {
                                                                              SyntaxKind.AddressOfExpression,
                                                                              SyntaxKind.BitwiseNotExpression,
                                                                              SyntaxKind.IndexExpression,
                                                                              SyntaxKind.LogicalNotExpression,
                                                                              SyntaxKind.PointerIndirectionExpression,
                                                                              SyntaxKind.PreDecrementExpression,
                                                                              SyntaxKind.PreIncrementExpression,
                                                                              SyntaxKind.UnaryMinusExpression,
                                                                              SyntaxKind.UnaryPlusExpression,
                                                                          };

        private static readonly SyntaxKind[] PostfixUnaryExpressionKinds =
                                                                           {
                                                                               SyntaxKind.PostDecrementExpression,
                                                                               SyntaxKind.PostIncrementExpression,
                                                                               SyntaxKind.SuppressNullableWarningExpression,
                                                                           };

        public MiKo_3504_TrainWreckAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            // statements
            context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeReturnStatement, SyntaxKind.ReturnStatement);
            context.RegisterSyntaxNodeAction(AnalyzeYieldReturnStatement, SyntaxKind.YieldReturnStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForEachVariableStatement, SyntaxKind.ForEachVariableStatement);
            context.RegisterSyntaxNodeAction(AnalyzeUsingStatement, SyntaxKind.UsingStatement);

            // assignments
            context.RegisterSyntaxNodeAction(AnalyzeArgument, SyntaxKind.Argument);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, AssignmentExpressionKinds);
            context.RegisterSyntaxNodeAction(AnalyzeEqualsValueClause, SyntaxKind.EqualsValueClause);

            // expressions
            context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression, BinaryExpressionKinds);
            context.RegisterSyntaxNodeAction(AnalyzePrefixUnaryExpression, PrefixUnaryExpressionKinds);
            context.RegisterSyntaxNodeAction(AnalyzePostfixUnaryExpression, PostfixUnaryExpressionKinds);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchExpression, SyntaxKind.SwitchExpression);
            context.RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);

            // patterns
            context.RegisterSyntaxNodeAction(AnalyzeIsPatternExpression, SyntaxKind.IsPatternExpression);

            // conditions
            context.RegisterSyntaxNodeAction(AnalyzePatternSwitchLabel, SyntaxKind.CasePatternSwitchLabel);
        }

        private static bool IsTrainWreckFinalMarker(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case ConditionalAccessExpressionSyntax _:
                case ElementAccessExpressionSyntax _:
                case IdentifierNameSyntax _:
                case InvocationExpressionSyntax _:
                case MemberAccessExpressionSyntax _:
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsTrainWreck(ExpressionSyntax syntax, SemanticModel semanticModel)
        {
            switch (syntax)
            {
                case ConditionalAccessExpressionSyntax conditional when IsTrainWreck(conditional, semanticModel):
                case ElementAccessExpressionSyntax access when IsTrainWreck(access, semanticModel):
                case InvocationExpressionSyntax invocation when IsTrainWreck(invocation, semanticModel):
                case MemberAccessExpressionSyntax member when IsTrainWreck(member, semanticModel):
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsTrainWreck(ConditionalAccessExpressionSyntax level1, SemanticModel semanticModel) => IsTrainWreck(level1, AllowedDepth, semanticModel);

        private static bool IsTrainWreck(ElementAccessExpressionSyntax level1, SemanticModel semanticModel) => IsTrainWreck(level1, AllowedDepth, semanticModel);

        private static bool IsTrainWreck(MemberAccessExpressionSyntax level1, SemanticModel semanticModel) => IsTrainWreck(level1, AllowedDepth, semanticModel);

        private static bool IsTrainWreck(InvocationExpressionSyntax level0, SemanticModel semanticModel) => IsTrainWreck(level0, AllowedDepth, semanticModel);

        private static bool IsTrainWreck(ExpressionSyntax start, in int allowedDepth, SemanticModel semanticModel)
        {
            var expressions = GetExpressions(start);

            if (expressions.Count <= allowedDepth)
            {
                return false;
            }

            var remainingExpressions = expressions.SkipWhile(_ => _.GetSymbol(semanticModel) is INamespaceOrTypeSymbol) // skip namespaces and types (fully qualified names)
                                                  .SkipWhere(MethodCanBeIgnored)
                                                  .SkipWhere(TypeCanBeIgnored);

            var problematicSyntax = remainingExpressions.ElementAtOrDefault(allowedDepth);

            if (problematicSyntax is null)
            {
                return false;
            }

            return IsTrainWreckFinalMarker(problematicSyntax);

            bool MethodCanBeIgnored(ExpressionSyntax syntax)
            {
                if (syntax.GetSymbol(semanticModel) is IMethodSymbol method)
                {
                    if (method.IsExtensionMethod())
                    {
                        // ignore extension methods as they are allowed to be chained
                        return true;
                    }

                    if (method.Name.StartsWith("With", StringComparison.Ordinal))
                    {
                        // ignore (Roslyn) methods that are defined as fluent API
                        return true;
                    }

                    // TODO RKN: Ignore others
                }

                return false;
            }

            bool TypeCanBeIgnored(ExpressionSyntax syntax)
            {
                if (syntax.GetTypeSymbol(semanticModel) is INamedTypeSymbol type)
                {
                    if (type.Name.AsSpan().EndsWith("Builder"))
                    {
                        // ignore builders
                        return true;
                    }

                    // ignore NUnit constraints
                    return type.ContainingNamespace is INamespaceSymbol ns && Constants.Names.AssertionNamespaces.Contains(ns.FullyQualifiedName());
                }

                return false;
            }
        }

        private static Stack<ExpressionSyntax> GetExpressions(ExpressionSyntax start)
        {
            var expressions = new Stack<ExpressionSyntax>();
            expressions.Push(start);

            var current = start;

            while (true)
            {
                var next = GetExpression(current);

                if (next is null)
                {
                    break;
                }

                expressions.Push(next);

                current = next;
            }

            return expressions;
        }

        private static ExpressionSyntax GetExpression(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case ConditionalAccessExpressionSyntax c:
                {
                    return c.WhenNotNull;
                }

                case MemberAccessExpressionSyntax m:
                {
                    if (m.Expression is ElementAccessExpressionSyntax next)
                    {
                        // jump over the access as we have something like 'A[42]' which we count as a single train car
                        return next.Expression;
                    }

                    return m.Expression;
                }

                case ElementAccessExpressionSyntax e:
                {
                    return e.Expression;
                }

                case InvocationExpressionSyntax i:
                {
                    if (i.Expression is MemberAccessExpressionSyntax next)
                    {
                        if (next.Expression is ElementAccessExpressionSyntax nextNext)
                        {
                            // jump over the access as we have something like 'A[42]' which we count as a single train car
                            return nextNext.Expression;
                        }

                        // jump over the member as that belongs to the invocation as well
                        return next.Expression;
                    }

                    return i.Expression;
                }

                default:
                    return null;
            }
        }

        private void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ExpressionStatementSyntax e)
            {
                ReportDiagnostics(context, e.Expression);
            }
        }

        private void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ArgumentSyntax a)
            {
                ReportDiagnostics(context, a.Expression);
            }
        }

        private void AnalyzeAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is AssignmentExpressionSyntax a)
            {
                ReportDiagnostics(context, a.Left);
                ReportDiagnostics(context, a.Right);
            }
        }

        private void AnalyzeEqualsValueClause(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is EqualsValueClauseSyntax e)
            {
                ReportDiagnostics(context, e.Value);
            }
        }

        private void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax b)
            {
                ReportDiagnostics(context, b.Left);
                ReportDiagnostics(context, b.Right);
            }
        }

        private void AnalyzePrefixUnaryExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is PrefixUnaryExpressionSyntax p)
            {
                ReportDiagnostics(context, p.Operand);
            }
        }

        private void AnalyzePostfixUnaryExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is PostfixUnaryExpressionSyntax p)
            {
                ReportDiagnostics(context, p.Operand);
            }
        }

        private void AnalyzeIsPatternExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IsPatternExpressionSyntax p)
            {
                ReportDiagnostics(context, p.Expression);
            }
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is SwitchStatementSyntax s)
            {
                ReportDiagnostics(context, s.Expression);
            }
        }

        private void AnalyzeSwitchExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is SwitchExpressionSyntax s)
            {
                ReportDiagnostics(context, s.GoverningExpression);
            }
        }

        private void AnalyzePatternSwitchLabel(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is CasePatternSwitchLabelSyntax c && c.WhenClause is WhenClauseSyntax when)
            {
                ReportDiagnostics(context, when.Condition);
            }
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IfStatementSyntax i)
            {
                ReportDiagnostics(context, i.Condition);
            }
        }

        private void AnalyzeYieldReturnStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is YieldStatementSyntax y)
            {
                ReportDiagnostics(context, y.Expression);
            }
        }

        private void AnalyzeReturnStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ReturnStatementSyntax r)
            {
                ReportDiagnostics(context, r.Expression);
            }
        }

        private void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ConditionalExpressionSyntax c)
            {
                ReportDiagnostics(context, c.Condition);
                ReportDiagnostics(context, c.WhenTrue);
                ReportDiagnostics(context, c.WhenFalse);
            }
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is WhileStatementSyntax w)
            {
                ReportDiagnostics(context, w.Condition);
            }
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DoStatementSyntax d)
            {
                ReportDiagnostics(context, d.Condition);
            }
        }

        private void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ForEachStatementSyntax f)
            {
                ReportDiagnostics(context, f.Expression);
            }
        }

        private void AnalyzeForEachVariableStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ForEachVariableStatementSyntax f)
            {
                ReportDiagnostics(context, f.Expression);
            }
        }

        private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is UsingStatementSyntax u)
            {
                ReportDiagnostics(context, u.Expression);
            }
        }

        private void ReportDiagnostics(in SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            if (IsTrainWreck(expression, context.SemanticModel))
            {
                while (expression is InvocationExpressionSyntax i)
                {
                    expression = i.Expression; // avoid the arguments being underlined
                }

                ReportDiagnostics(context, Issue(expression));
            }
        }
    }
}