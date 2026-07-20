using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3111_TestAssertsUseSpecificConstraintInsteadOfEqualToAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3111";

        private const string Empty = "Empty";
        private const string NaN = "NaN";
        private const string Negative = "Negative";
        private const string Positive = "Positive";

        private static readonly ConcurrentDictionary<string, string> ConstraintMap = new ConcurrentDictionary<string, string>(new Dictionary<string, string>
                                                                                                                                   {
                                                                                                                                       { "0", "Zero" },
                                                                                                                                       { "null", "Null" },
                                                                                                                                       { "true", "True" },
                                                                                                                                       { "false", "False" },
                                                                                                                                       { string.Empty, "Empty" },
                                                                                                                                   });

        public MiKo_3111_TestAssertsUseSpecificConstraintInsteadOfEqualToAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpressionSyntax, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocationExpressionSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var issues = Analyze(node);

            if (issues.Length > 0)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private Diagnostic[] Analyze(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax maes)
            {
                var argumentList = node.ArgumentList;
                var arguments = argumentList.Arguments;

                if (arguments.Count is 1)
                {
                    switch (maes.Name.GetName())
                    {
                        case "EqualTo":
                        {
                            switch (arguments[0].Expression)
                            {
                                case LiteralExpressionSyntax literal when ConstraintMap.TryGetValue(literal.Token.ValueText, out var constraint):
                                    return Issue(constraint);

                                case MemberAccessExpressionSyntax m when m.GetName() is NaN:
                                    return Issue(NaN);

                                case MemberAccessExpressionSyntax m when m.GetName() is Empty && (m.GetIdentifierName() is "string" || m.GetIdentifierName() is "String"):
                                    return Issue(Empty);
                            }

                            break;
                        }

                        case "LessThan" when arguments[0].Expression is LiteralExpressionSyntax l && l.Token.ValueText is "0":
                            return Issue(Negative);

                        case "GreaterThan" when arguments[0].Expression is LiteralExpressionSyntax g && g.Token.ValueText is "0":
                            return Issue(Positive);
                    }
                }

                Diagnostic[] Issue(string value)
                {
                    var location = node.GetLocation(maes.Name.SpanStart, argumentList.Span.End);

                    return new[] { Issue<string>(location, value, new Pair(Constants.AnalyzerCodeFixSharedData.NUnitReplacement, value)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}