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

            if (method.IsOverride || method.IsVirtual)
                return true;

            if (method.IsConstructor())
                return method.IsSerializationConstructor();

            if (method.IsEventHandler())
                return true;

            if (method.IsDependencyObjectEventHandler())
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
            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;

            Analyze(context, methodBody, parameters);
        }

        private void AnalyzeConstructor(SyntaxNodeAnalysisContext context)
        {
            var method = (ConstructorDeclarationSyntax)context.Node;

            var parameters = method.ParameterList.Parameters;
            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;

            Analyze(context, methodBody, parameters);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode methodBody, SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            if (methodBody is null)
                return;

            if (parameters.Count == 0)
                return;

            var methodSymbol = context.GetEnclosingMethod();

            if (CanBeIgnored(methodSymbol))
                return;

            var used = GetAllUsedVariables(context, methodBody);

            foreach (var parameter in parameters)
            {
                var parameterName = parameter.Identifier.ValueText;

                if (used.Contains(parameterName))
                    continue;

                // TODO: RKN check if the documentation contains the phrase 'Unused.' and Do not report an issue in such case
                if (methodSymbol.IsEnhancedByPostSharpAdvice())
                    continue;

                var diagnostic = Issue(parameterName, parameter.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
