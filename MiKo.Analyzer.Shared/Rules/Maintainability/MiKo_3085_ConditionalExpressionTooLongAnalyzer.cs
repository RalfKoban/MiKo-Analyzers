using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3085_ConditionalExpressionTooLongAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3085";

        private const int MaxExpressionLength = 60;

        private static readonly SyntaxKind[] IsExpressions =
                                                             {
                                                                 SyntaxKind.IsPatternExpression,
                                                                 SyntaxKind.IsExpression,
                                                             };

        public MiKo_3085_ConditionalExpressionTooLongAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.ConditionalExpression);

        private static bool ObjectCreationCannotBeShortened(ObjectCreationExpressionSyntax syntax, SemanticModel semanticModel)
        {
            var argumentList = syntax.ArgumentList;

            if (argumentList is null)
            {
                // ignore object initializers
                return true;
            }

            var arguments = argumentList.Arguments;

            if (arguments.Count == 0)
            {
                // ignore as it cannot be shorted anymore
                return true;
            }

            // inspect arguments to see only simple ones
            var found = arguments
                            .Select(_ => _.Expression)
                            .All(_ =>
                                 {
                                     switch (_)
                                     {
                                         case LiteralExpressionSyntax _:
                                         case MemberAccessExpressionSyntax m when m.IsKind(SyntaxKind.SimpleMemberAccessExpression):
                                         case IdentifierNameSyntax i when i.Identifier.GetSymbol(semanticModel) is IParameterSymbol:
                                             return true;

                                         default:
                                             return false;
                                     }
                                 });

            return found;
        }

        private static bool InvocationCannotBeShortened(InvocationExpressionSyntax syntax)
        {
            switch (syntax.Expression)
            {
                case MemberAccessExpressionSyntax m when m.IsKind(SyntaxKind.SimpleMemberAccessExpression):
                    return SimpleMemberAccessCannotBeShortened(m);

                case IdentifierNameSyntax n when n.GetName() == nameof(GetHashCode):
                    return true; // we assume that any GetHashCode invocation cannot be shortened anymore

                default:
                    return false;
            }
        }

        private static bool SimpleMemberAccessCannotBeShortened(MemberAccessExpressionSyntax m)
        {
            var name = m.GetName();

            if (name.StartsWith("Try", StringComparison.Ordinal))
            {
                return true;
            }

            switch (name)
            {
                case nameof(Enumerable.Empty) when m.Expression is IdentifierNameSyntax i:
                {
                    switch (i.GetName())
                    {
                        case nameof(Array):
                        case nameof(Enumerable):
                            return true;

                        default:
                            return false;
                    }
                }

                case nameof(string.IsNullOrEmpty):
                case nameof(string.IsNullOrWhiteSpace):
                    return true;

                default:
                    return false;
            }
        }

        private static bool AnalyzeLength(SyntaxNode node, SemanticModel semanticModel)
        {
            if (node.Span.Length <= MaxExpressionLength)
            {
                return false;
            }

            if (node.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                // ignore as it cannot be shorted anymore
                return false;
            }

            switch (node)
            {
                case InterpolatedStringExpressionSyntax _:
                case ObjectCreationExpressionSyntax o when ObjectCreationCannotBeShortened(o, semanticModel):
                case InvocationExpressionSyntax i when InvocationCannotBeShortened(i):
                    return false; // ignore as it cannot be shorted anymore

                case PrefixUnaryExpressionSyntax logic when logic.Operand is InvocationExpressionSyntax i && InvocationCannotBeShortened(i):
                    return false; // ignore as it cannot be shorted anymore (it could be refactored but that does not shorten it enough)

                default:
                    return true;
            }
        }

        private void AnalyzeLength(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (AnalyzeLength(node, context.SemanticModel))
            {
                ReportDiagnostics(context, Issue(string.Empty, node));
            }
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ConditionalExpressionSyntax)context.Node;

            if (node.Condition.IsAnyKind(IsExpressions))
            {
                // ignore
            }
            else
            {
                AnalyzeLength(context, node.Condition);
            }

            AnalyzeLength(context, node.WhenTrue);
            AnalyzeLength(context, node.WhenFalse);
        }
    }
}