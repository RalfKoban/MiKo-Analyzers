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
    public sealed class MiKo_3038_DoNotUseMagicNumbersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3038";

        private static readonly HashSet<string> WellKnownNumbers = new HashSet<string>
                                                                       {
                                                                           // ignore zero
                                                                           "0",
                                                                           "0l",
                                                                           "0u",
                                                                           "0d",
                                                                           "0f",
                                                                           "0.0",
                                                                           "0.0f",
                                                                           "0.0d",

                                                                           // ignore one as it is often used as offset
                                                                           "1",
                                                                           "1l",
                                                                           "1u",
                                                                           "1d",
                                                                           "1f",
                                                                           "1.0",
                                                                           "1.0f",
                                                                           "1.0d",

                                                                           "320",
                                                                           "200",
                                                                           "640",
                                                                           "480",
                                                                           "800",
                                                                           "600",
                                                                           "1024",
                                                                           "768",
                                                                           "1920",
                                                                           "1080",
                                                                           "1280",
                                                                           "1440",
                                                                           "1600",
                                                                           "1200",
                                                                       };

        public MiKo_3038_DoNotUseMagicNumbersAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNumericLiteralExpression, SyntaxKind.NumericLiteralExpression);

        private static bool IgnoreBasedOnAncestor(LiteralExpressionSyntax node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case LocalDeclarationStatementSyntax variable when variable.Modifiers.Any(_ => _.IsKind(SyntaxKind.ConstKeyword)):
                    case FieldDeclarationSyntax field when field.Modifiers.Any(_ => _.IsKind(SyntaxKind.ConstKeyword)):
                    case EnumMemberDeclarationSyntax _:
                    case AttributeArgumentSyntax _:
                    case PragmaWarningDirectiveTriviaSyntax _:
                        return true;
                }
            }

            return false;
        }

        private static bool IgnoreBasedOnParent(LiteralExpressionSyntax node)
        {
            var parent = node.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    return false;

                case SyntaxKind.CaseSwitchLabel:
                case SyntaxKind.RangeExpression:
                    return true;

                case SyntaxKind.Argument:
                    return IgnoreBasedOnArgument(parent); // we want to know what those numbers mean

                case SyntaxKind.SimpleAssignmentExpression:
                    return false; // assignments to width and height (???)

                case SyntaxKind.ArrayRankSpecifier:
                    return false; // arrays, such as new byte[123]

                default:
                    return false;
            }
        }

        private static bool IgnoreBasedOnArgument(SyntaxNode node)
        {
            if (node.Parent is ArgumentListSyntax list)
            {
                switch (list.Parent)
                {
                    case ObjectCreationExpressionSyntax o:
                    {
                        var name = o.Type.GetNameOnlyPart();

                        switch (name)
                        {
                            case nameof(DateTime):
                            case nameof(DateTimeOffset):
                            case nameof(TimeSpan):
                            {
                                const int MinimumArgumentsForHoursMinutesSeconds = 3;

                                return list.Arguments.Count >= MinimumArgumentsForHoursMinutesSeconds;
                            }

                            case nameof(Version):
                            {
                                // ignore version ctors
                                return true;
                            }

                            default:
                            {
                                // ignore progress ctors
                                return name.Contains("Progress");
                            }
                        }
                    }

                    case InvocationExpressionSyntax i:
                    {
                        var name = i.Expression.GetName();

                        if (i.Expression is MemberAccessExpressionSyntax)
                        {
                            if (name.StartsWith("From", StringComparison.Ordinal))
                            {
                                var typeName = i.GetName();

                                switch (typeName)
                                {
                                    case "Color": // ignore all Color.FromXyz calls
                                    case nameof(TimeSpan): // ignore all TimeSpan.FromXyz calls
                                        return true;
                                }
                            }

                            return false;
                        }

                        // ignore progress
                        return name.Contains("Progress");
                    }
                }
            }

            return false;
        }

        private static bool IsWellKnownNumber(string number) => WellKnownNumbers.Contains(number.ToLowerCase());

        private void AnalyzeNumericLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (LiteralExpressionSyntax)context.Node;

            ReportDiagnostics(context, AnalyzeNumericLiteralExpression(node, context.ContainingSymbol));
        }

        private IEnumerable<Diagnostic> AnalyzeNumericLiteralExpression(LiteralExpressionSyntax node, ISymbol symbol)
        {
            if (symbol is null)
            {
                yield break;
            }

            if (symbol is IMethodSymbol method)
            {
                if (method.Name == nameof(GetHashCode))
                {
                    // ignore hash calculation
                    yield break;
                }

                if (method.IsTestMethod())
                {
                    // ignore unit tests
                    yield break;
                }
            }

            var containingType = symbol.ContainingType;

            if (containingType != null)
            {
                if (containingType.IsTestClass())
                {
                    // ignore unit tests
                    yield break;
                }

                if (containingType.ContainingType?.IsTestClass() is true)
                {
                    // ignore nested types in unit tests
                    yield break;
                }
            }

            var number = node.Token.Text;

            if (IsWellKnownNumber(number))
            {
                yield break;
            }

            if (IgnoreBasedOnParent(node) || IgnoreBasedOnAncestor(node))
            {
                yield break;
            }

            yield return Issue(symbol.Name, node, number);
        }
    }
}