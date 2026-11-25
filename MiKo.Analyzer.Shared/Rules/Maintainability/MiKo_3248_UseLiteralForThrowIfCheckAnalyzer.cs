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
    public sealed class MiKo_3248_UseLiteralForThrowIfCheckAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3248";

        public MiKo_3248_UseLiteralForThrowIfCheckAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private static bool HasIssue(ArgumentSyntax argument) => argument.DescendantNodes<ExpressionSyntax>().Any(HasIssue);

        private static bool HasIssue(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case AnonymousMethodExpressionSyntax _:
                case AnonymousObjectCreationExpressionSyntax _:
                case ArrayCreationExpressionSyntax _:
                case AssignmentExpressionSyntax _:
                case AwaitExpressionSyntax _:
                case BaseObjectCreationExpressionSyntax _:
                case BinaryExpressionSyntax _:
                case CastExpressionSyntax _:
                case ConditionalAccessExpressionSyntax _:
                case ConditionalExpressionSyntax _:
                case ImplicitArrayCreationExpressionSyntax _:
                case InterpolatedStringExpressionSyntax _:
                case IsPatternExpressionSyntax _:
                case LambdaExpressionSyntax _:
                case PostfixUnaryExpressionSyntax _:
                case QueryExpressionSyntax _:
                case RangeExpressionSyntax _:
                case SwitchExpressionSyntax _:
                case TupleExpressionSyntax _:
                case WithExpressionSyntax _:
                    return true;

#if VS2022 || VS2026
                case CollectionExpressionSyntax _:
                    return true;
#endif

                case InvocationExpressionSyntax i when i.IsNameOf():
                    return false; // accept nameof

                case PrefixUnaryExpressionSyntax prefix when prefix.Operand.IsKind(SyntaxKind.NumericLiteralExpression):
                {
                    if (prefix.IsKind(SyntaxKind.UnaryMinusExpression) || prefix.IsKind(SyntaxKind.UnaryPlusExpression))
                    {
                        return false;
                    }

                    return true;
                }

                case InvocationExpressionSyntax _:
                case PrefixUnaryExpressionSyntax _:
                    return true;

                case ParenthesizedExpressionSyntax p:
                    return HasIssue(p.Expression);
                default:
                    return false;
            }
        }

        private static ArgumentSyntax FindArgument(in SeparatedSyntaxList<ArgumentSyntax> arguments, string s, in int index) => arguments.FirstOrDefault(_ => _.NameColon.GetName() == s) ?? arguments[index];

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation)
            {
                var issues = AnalyzeInvocation(invocation);

                if (issues.Length > 0)
                {
                    ReportDiagnostics(context, issues);
                }
            }
        }

        private Diagnostic[] AnalyzeInvocation(InvocationExpressionSyntax invocation)
        {
            var arguments = invocation.ArgumentList.Arguments;

            if (arguments.Count > 0)
            {
                switch (invocation.GetIdentifierName())
                {
                    case nameof(ArgumentException):
                    case nameof(ArgumentNullException):
                    case nameof(ArgumentOutOfRangeException):
                    {
                        switch (invocation.GetName())
                        {
                            case "ThrowIfNull":
                            case "ThrowIfNullOrEmpty":
                            case "ThrowIfNullOrWhiteSpace":
                            {
                                var argument = FindArgument(arguments, "argument", 0);

                                return HasIssue(argument)
                                       ? new[] { Issue(argument) }
                                       : Array.Empty<Diagnostic>();
                            }

                            case "ThrowIfNegative":
                            case "ThrowIfNegativeOrZero":
                            case "ThrowIfZero":
                            {
                                var argument = FindArgument(arguments, "value", 0);

                                return HasIssue(argument)
                                       ? new[] { Issue(argument) }
                                       : Array.Empty<Diagnostic>();
                            }

                            case "ThrowIfEqual":
                            case "ThrowIfNotEqual":
                            case "ThrowIfGreaterThan":
                            case "ThrowIfGreaterThanOrEqual":
                            case "ThrowIfLessThan":
                            case "ThrowIfLessThanOrEqual":
                            {
                                var argument0 = FindArgument(arguments, "value", 0);

                                var issues = new List<Diagnostic>(2);

                                if (HasIssue(argument0))
                                {
                                    issues.Add(Issue(argument0));
                                }

                                if (arguments.Count > 1)
                                {
                                    var argument1 = FindArgument(arguments, "other", 1);

                                    if (HasIssue(argument1))
                                    {
                                        issues.Add(Issue(argument1));
                                    }
                                }

                                return issues.ToArray();
                            }
                        }

                        break;
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}