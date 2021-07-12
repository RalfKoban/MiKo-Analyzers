using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5003_LogExceptionAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5003";

        public MiKo_5003_LogExceptionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

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
            var numberOfParameters = node.ArgumentList.Arguments.Count;

            if (numberOfParameters > 0 && node.Expression is MemberAccessExpressionSyntax methodCall)
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
                        if (numberOfParameters == 1)
                        {
                            return AnalyzeNonFormatCall(methodCall, node.ArgumentList.Arguments[0], semanticModel, methodName);
                        }

                        break;
                    }

                    case Constants.ILog.DebugFormat:
                    case Constants.ILog.InfoFormat:
                    case Constants.ILog.WarnFormat:
                    case Constants.ILog.ErrorFormat:
                    case Constants.ILog.FatalFormat:
                    {
                        return AnalyzeFormatCall(methodCall, node.ArgumentList.Arguments, semanticModel, methodName.Without("Format"));
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeNonFormatCall(MemberAccessExpressionSyntax methodCall, ArgumentSyntax argument, SemanticModel semanticModel, string proposedMethodName)
        {
            // check for correct type (only ILog methods shall be reported)
            var type = methodCall.GetTypeSymbol(semanticModel);

            if (type.Name == Constants.ILog.TypeName && argument.IsException(semanticModel))
            {
                var enclosingMethod = methodCall.GetEnclosingMethod(semanticModel);

                return Issue(enclosingMethod.Name, methodCall.Name, proposedMethodName);
            }

            return null;
        }

        private Diagnostic AnalyzeFormatCall(MemberAccessExpressionSyntax methodCall, IEnumerable<ArgumentSyntax> arguments, SemanticModel semanticModel, string proposedMethodName)
        {
            // check for correct type (only ILog methods shall be reported)
            var type = methodCall.GetTypeSymbol(semanticModel);

            if (type.Name == Constants.ILog.TypeName && arguments.Any(_ => _.IsException(semanticModel)))
            {
                var enclosingMethod = methodCall.GetEnclosingMethod(semanticModel);

                return Issue(enclosingMethod.Name, methodCall.Name, proposedMethodName);
            }

            return null;
        }
    }
}