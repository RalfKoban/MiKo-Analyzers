﻿using System;
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

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName(Constants.ILog.FullTypeName) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;
            var issue = AnalyzeInvocation(node, context.SemanticModel);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeInvocation(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            var arguments = node.ArgumentList.Arguments;

            if (arguments.Count is 0)
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
                        return arguments.Count is 1
                               ? AnalyzeCall(methodCall, arguments, semanticModel, methodName)
                               : null;
                    }

                    case Constants.ILog.DebugFormat:
                    case Constants.ILog.InfoFormat:
                    case Constants.ILog.WarnFormat:
                    case Constants.ILog.ErrorFormat:
                    case Constants.ILog.FatalFormat:
                    {
                        return AnalyzeCall(methodCall, arguments, semanticModel, methodName.Without("Format"));
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeCall(MemberAccessExpressionSyntax methodCall, in SeparatedSyntaxList<ArgumentSyntax> arguments, SemanticModel semanticModel, string proposedMethodName)
        {
            // only ILog methods shall be reported
            var type = methodCall.GetTypeSymbol(semanticModel);

            // it may happen that in some broken code Roslyn is unable to detect a type (e.g. due to missing code paths), hence 'type' could be null here
            if (type?.Name is Constants.ILog.TypeName && arguments.Any(_ => _.IsException(semanticModel)))
            {
                var enclosingMethod = methodCall.GetEnclosingMethod(semanticModel);

                return Issue(enclosingMethod.Name, methodCall.Name, proposedMethodName);
            }

            return null;
        }
    }
}