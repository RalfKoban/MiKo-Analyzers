using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3062_ExceptionLogMessageEndsWithColonAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3062";

        public MiKo_3062_ExceptionLogMessageEndsWithColonAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            var diagnostic = AnalyzeInvocation(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
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
                        if (arguments.Any(_ => _.IsException(semanticModel)))
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
                        if (arguments.Any(_ => _.IsException(semanticModel)))
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

            if (type.Name == Constants.ILog.TypeName)
            {
                switch (argument.Expression)
                {
                    case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                        return AnalyzeToken(literal.Token);

                    case InterpolatedStringExpressionSyntax i when i.Contents.Last() is InterpolatedStringTextSyntax interpolated:
                        return AnalyzeToken(interpolated.TextToken);

                    case InterpolatedStringExpressionSyntax i:
                        return Issue(i);
                }
            }

            return null;
        }

        private Diagnostic AnalyzeToken(SyntaxToken token) => token.ValueText.TrimEnd().EndsWith(':')
                                                                  ? null
                                                                  : Issue(token);
    }
}