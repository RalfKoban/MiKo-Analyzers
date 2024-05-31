using System;
using System.Collections.Generic;

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

                                                                           // ignore screen resolutions
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

        private static readonly HashSet<string> Twos = new HashSet<string>
                                                           {
                                                               "2",
                                                               "2l",
                                                               "2u",
                                                               "2.0",
                                                               "2.0d",
                                                               "2.0f",
                                                           };

        public MiKo_3038_DoNotUseMagicNumbersAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNumericLiteralExpression, SyntaxKind.NumericLiteralExpression);

        private static bool IgnoreBasedOnAncestor(LiteralExpressionSyntax node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.AttributeArgument:
                    case SyntaxKind.EnumMemberDeclaration:
                    case SyntaxKind.PragmaWarningDirectiveTrivia:
                        return true;

                    case SyntaxKind.LocalDeclarationStatement:
                        return ((LocalDeclarationStatementSyntax)ancestor).IsConst;

                    case SyntaxKind.FieldDeclaration:
                        return ((FieldDeclarationSyntax)ancestor).IsConst();
                }
            }

            return false;
        }

        private static bool IgnoreBasedOnParent(LiteralExpressionSyntax node)
        {
            var parent = FilterUnimportantParents(node);
            var kind = parent?.Kind();

            switch (kind)
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    return false;

                case SyntaxKind.ArrayRankSpecifier:
                    return false; // arrays, such as new byte[123]

                case SyntaxKind.SimpleAssignmentExpression:
                    return true; // assignments to width and height (???)

                case SyntaxKind.CaseSwitchLabel:
                case SyntaxKind.RangeExpression:
                    return true;

                case SyntaxKind.Argument:
                    return IgnoreBasedOnArgument(parent); // we want to know what those numbers mean

                case SyntaxKind.DivideAssignmentExpression when IsTwo(node):
                    return true;

                case SyntaxKind.DivideExpression when IsTwo(node):
                    return true;

                case SyntaxKind.EqualsValueClause when parent.Parent is VariableDeclaratorSyntax:
                    return true;

                // ignore enums
                case SyntaxKind.EqualsValueClause when parent.Parent is EnumMemberDeclarationSyntax:
                    return true;

                // ignore property initializers
                case SyntaxKind.EqualsValueClause when parent.Parent is BasePropertyDeclarationSyntax:
                    return true;

                // ignore getter properties that return only a value
                case SyntaxKind.ReturnStatement when parent.Parent is BlockSyntax block && block.Parent is AccessorDeclarationSyntax accessor && accessor.IsKind(SyntaxKind.GetAccessorDeclaration):
                    return true;

                // ignore expression body properties that directly return a value
                case SyntaxKind.ArrowExpressionClause when parent.Parent is BasePropertyDeclarationSyntax:
                    return true;

                // ignore properties with expression body getter that directly return a value
                case SyntaxKind.ArrowExpressionClause when parent.Parent is AccessorDeclarationSyntax accessor && accessor.IsKind(SyntaxKind.GetAccessorDeclaration):
                    return true;

                default:
                    return false;
            }
        }

        private static SyntaxNode FilterUnimportantParents(LiteralExpressionSyntax node)
        {
            var parent = FilterUnimportantUnaryParents(node.Parent);

            if (parent != null && parent.IsKind(SyntaxKind.ParenthesizedExpression))
            {
                parent = parent?.Parent;
            }

            return parent;
        }

        private static SyntaxNode FilterUnimportantUnaryParents(SyntaxNode parent)
        {
            switch (parent?.Kind())
            {
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.UnaryPlusExpression:
                    return parent?.Parent;

                default:
                    return parent;
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
                        var name = o.Type.GetName(); // .GetNameOnlyPart();

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

                            case "ArrayList":
                            case "List":
                            case "Dictionary":
                            case "HashSet":
                            case "Stack":
                            case "Queue":
                            {
                                // ignore list creations
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

        private static bool IsTwo(LiteralExpressionSyntax syntax) => Twos.Contains(syntax.Token.Text.ToLowerCase());

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

            if (node.Parent is PrefixUnaryExpressionSyntax prefix && prefix.IsKind(SyntaxKind.UnaryMinusExpression))
            {
                yield return Issue(symbol.Name, prefix, "-" + number);
            }
            else
            {
                yield return Issue(symbol.Name, node, number);
            }
        }
    }
}