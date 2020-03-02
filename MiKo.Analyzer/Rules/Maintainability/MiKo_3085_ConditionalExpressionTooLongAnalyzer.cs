﻿using System.Linq;

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

        private const int MaxExpressionLength = 35;

        public MiKo_3085_ConditionalExpressionTooLongAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, SyntaxKind.ConditionalExpression);

        private static bool ObjectCreationCannotBeShortened(ObjectCreationExpressionSyntax syntax, SemanticModel semanticModel)
        {
            var arguments = syntax.ArgumentList.Arguments;
            if (arguments.Count == 0)
            {
                // ignore as it cannot be shorted anymore
                return true;
            }

            // inspect arguments to see only simple ones
            var found = arguments
                            .Select(_ => _.Expression)
                            .All(
                                 _ =>
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

            if (node is ObjectCreationExpressionSyntax o && ObjectCreationCannotBeShortened(o, semanticModel))
            {
                // ignore as it cannot be shorted anymore
                return false;
            }

            return true;
        }

        private void AnalyzeLength(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (AnalyzeLength(node, context.SemanticModel))
            {
                context.ReportDiagnostic(Issue(string.Empty, node));
            }
        }

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ConditionalExpressionSyntax)context.Node;

            AnalyzeLength(context, node.Condition);
            AnalyzeLength(context, node.WhenTrue);
            AnalyzeLength(context, node.WhenFalse);
        }
    }
}