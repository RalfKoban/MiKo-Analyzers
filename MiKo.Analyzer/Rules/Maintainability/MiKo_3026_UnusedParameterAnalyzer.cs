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
    public sealed class MiKo_3026_UnusedParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3026";

        public MiKo_3026_UnusedParameterAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructor, SyntaxKind.ConstructorDeclaration);
        }

        private static bool CanBeIgnored(IMethodSymbol method)
        {
            if (method is null)
                return false;

            if (method.IsOverride)
                return true;

            if (method.IsEventHandler())
                return true;

            var ignore = method.IsInterfaceImplementation();
            return ignore;
        }

        private static HashSet<string> GetAllUsedVariables(SyntaxNodeAnalysisContext context, SyntaxNode statementOrExpression)
        {
            var dataFlow = context.SemanticModel.AnalyzeDataFlow(statementOrExpression);

            // do not use the declared ones as we are interested in parameters, not unused variables
            // var variablesDeclared = dataFlow.VariablesDeclared;

            var variablesRead = dataFlow.ReadInside.Union(dataFlow.ReadOutside);

            // do not include the ones that are written outside as those are the ones that are not used at all
            var variablesWritten = dataFlow.WrittenInside;

            var used = variablesRead.Union(variablesWritten).Select(_ => _.Name).ToHashSet();
            return used;
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            var parameters = method.ParameterList.Parameters;
            if (parameters.Count == 0)
                return;

            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;
            if (methodBody is null)
                return; // unfinished code or code that has no body (such as interface methods or abstract methods)

            var methodSymbol = method.GetEnclosingMethod(context.SemanticModel);
            if (CanBeIgnored(methodSymbol))
                return;

            var results = Analyze(context, methodBody, parameters);
            foreach (var diagnostic in results)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeConstructor(SyntaxNodeAnalysisContext context)
        {
            var method = (ConstructorDeclarationSyntax)context.Node;

            var parameters = method.ParameterList.Parameters;
            if (parameters.Count == 0)
                return;

            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;
            if (methodBody is null)
                return; // unfinished code or code that has no body (such as interface methods or abstract methods)

            var results = Analyze(context, methodBody, parameters);
            foreach (var diagnostic in results)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> Analyze(SyntaxNodeAnalysisContext context, SyntaxNode methodBody, SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            var used = GetAllUsedVariables(context, methodBody);

            var results = new List<Diagnostic>();
            foreach (var parameter in parameters)
            {
                var parameterName = parameter.Identifier.ValueText;

                if (used.Contains(parameterName))
                    continue;

                var diagnostic = ReportIssue(parameterName, parameter.GetLocation());
                results.Add(diagnostic);
            }

            return results;
        }
    }
}
