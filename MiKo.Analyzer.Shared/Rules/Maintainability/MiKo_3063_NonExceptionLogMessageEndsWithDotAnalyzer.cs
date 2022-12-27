using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3063_NonExceptionLogMessageEndsWithDotAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3063";

        public MiKo_3063_NonExceptionLogMessageEndsWithDotAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName(Constants.ILog.FullTypeName) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;
            var issue = AnalyzeInvocation(node, context.SemanticModel);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeInvocation(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            var arguments = node.ArgumentList.Arguments;

            if (arguments.Count == 0)
            {
                return null;
            }

            if (node.Expression is MemberAccessExpressionSyntax methodCall)
            {
                var methodName = methodCall.GetName();

                switch (methodName)
                {
                    case Constants.ILog.Debug:
                    case Constants.ILog.Info:
                    case Constants.ILog.Warn:
                    case Constants.ILog.Error:
                    case Constants.ILog.Fatal:
                    {
                        if (arguments.None(_ => _.IsException(semanticModel)))
                        {
                            if (arguments[0].IsStringLiteral())
                            {
                                return AnalyzeCall(methodCall, arguments[0], semanticModel);
                            }
                        }

                        break;
                    }

                    case Constants.ILog.DebugFormat:
                    case Constants.ILog.InfoFormat:
                    case Constants.ILog.WarnFormat:
                    case Constants.ILog.ErrorFormat:
                    case Constants.ILog.FatalFormat:
                    {
                        if (arguments.None(_ => _.IsException(semanticModel)))
                        {
                            if (arguments[0].IsStringLiteral())
                            {
                                return AnalyzeCall(methodCall, arguments[0], semanticModel);
                            }

                            // TODO: Find correct argument, especially for those with 3 or 4 parameters
                            if (arguments.Count > 1 && arguments[1].IsStringLiteral())
                            {
                                return AnalyzeCall(methodCall, arguments[1], semanticModel);
                            }
                        }

                        break;
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeCall(MemberAccessExpressionSyntax methodCall, ArgumentSyntax argument, SemanticModel semanticModel)
        {
            // only ILog methods shall be reported
            var type = methodCall.GetTypeSymbol(semanticModel);

            // it may happen that in some broken code Roslyn is unable to detect a type (eg. due to missing code paths), hence 'type' could be null here
            if (type?.Name == Constants.ILog.TypeName)
            {
                switch (argument.Expression)
                {
                    case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression) && literal.Token.ValueText.TrimEnd().EndsWithAny(".:") is false:
                    {
                        return CreateIssue(literal.Token);
                    }

                    case InterpolatedStringExpressionSyntax i:
                    {
                        if (i.Contents.Last() is InterpolatedStringTextSyntax interpolated && interpolated.TextToken.ValueText.TrimEnd().EndsWithAny(".:"))
                        {
                            // nothing to report
                            return null;
                        }

                        return CreateIssue(i);
                    }
                }
            }

            return null;
        }

        private Diagnostic CreateIssue(SyntaxNode node)
        {
            var end = node.Span.End;
            var start = Math.Max(node.SpanStart, end - 2); // we want to underline the last 2 characters
            var location = CreateLocation(node, start, end);

            return Issue(location);
        }

        private Diagnostic CreateIssue(SyntaxToken token)
        {
            var end = token.Span.End;
            var start = Math.Max(token.SpanStart, end - 2); // we want to underline the last 2 characters
            var location = CreateLocation(token, start, end);

            return Issue(location);
        }
    }
}