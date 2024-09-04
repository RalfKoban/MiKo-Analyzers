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

        private static readonly SyntaxKind[] PossibleEventRegistrations = { SyntaxKind.AddAssignmentExpression, SyntaxKind.SubtractAssignmentExpression };

        public MiKo_3026_UnusedParameterAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructor, SyntaxKind.ConstructorDeclaration);
        }

        private static bool CanBeIgnored(IMethodSymbol method)
        {
            if (method is null)
            {
                return false;
            }

            if (method.IsOverride || method.IsVirtual)
            {
                return true;
            }

            if (method.IsPartial())
            {
                return true;
            }

            if (method.IsConstructor())
            {
                return method.IsSerializationConstructor();
            }

            if (method.IsEventHandler())
            {
                return true;
            }

            if (method.IsDependencyPropertyEventHandler())
            {
                return true;
            }

            if (method.IsDependencyObjectEventHandler())
            {
                return true;
            }

            if (method.IsCoerceValueCallback())
            {
                return true;
            }

            if (method.IsValidateValueCallback())
            {
                return true;
            }

            if (method.IsAspNetCoreStartUp())
            {
                return true;
            }

            if (method.IsAspNetCoreController())
            {
                return true;
            }

            // TODO: RKN check if the documentation contains the phrase 'Unused.' and Do not report an issue in such case
            if (method.IsEnhancedByPostSharpAdvice())
            {
                return true;
            }

            var ignore = method.IsInterfaceImplementation();

            return ignore;
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            var parameters = method.ParameterList.Parameters;
            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;

            Analyze(context, methodBody, parameters, method.GetName());
        }

        private void AnalyzeConstructor(SyntaxNodeAnalysisContext context)
        {
            var ctor = (ConstructorDeclarationSyntax)context.Node;

            var parameters = ctor.ParameterList.Parameters;
            var methodBody = ctor.Body ?? (SyntaxNode)ctor.ExpressionBody?.Expression;

            Analyze(context, methodBody, parameters);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode methodBody, SeparatedSyntaxList<ParameterSyntax> parameters, string methodName = null)
        {
            if (methodBody is null)
            {
                return;
            }

            if (parameters.Count == 0)
            {
                return;
            }

            var methodSymbol = context.GetEnclosingMethod();

            if (CanBeIgnored(methodSymbol))
            {
                return;
            }

            var issues = AnalyzeParameters(methodName, methodBody, parameters, context.SemanticModel);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeParameters(string methodName, SyntaxNode methodBody, SeparatedSyntaxList<ParameterSyntax> parameters, SemanticModel semanticModel)
        {
            var eventAssignments = new Lazy<List<AssignmentExpressionSyntax>>(CollectEventAssignments);
            var identifiersUsedAsArguments = new Lazy<ISet<string>>(CollectArgumentIdentifiers);

            var used = methodBody.GetAllUsedVariables(semanticModel);

            foreach (var parameter in parameters)
            {
                var parameterName = parameter.GetName();

                if (used.Contains(parameterName))
                {
                    continue;
                }

                // only check for methods with names as ctors cannot be registered for event handlers or callbacks
                if (methodName != null)
                {
                    if (eventAssignments.Value.Exists(_ => _.Right is IdentifierNameSyntax identifier && identifier.Identifier.ValueText == methodName))
                    {
                        // seems to be an assignment so we do not report that parameter
                        continue;
                    }

                    if (identifiersUsedAsArguments.Value.Contains(methodName))
                    {
                        // seems to be a callback
                        continue;
                    }
                }

                yield return Issue(parameterName, parameter.Identifier);
            }

            List<AssignmentExpressionSyntax> CollectEventAssignments()
            {
                var root = methodBody.SyntaxTree.GetCompilationUnitRoot();

                var assignments = root.DescendantNodes<AssignmentExpressionSyntax>(_ => _.IsAnyKind(PossibleEventRegistrations) && _.IsEventRegistration(semanticModel))
                                      .ToList();

                return assignments;
            }

            ISet<string> CollectArgumentIdentifiers()
            {
                var root = methodBody.SyntaxTree.GetCompilationUnitRoot();

                var identifiers = root.DescendantNodes<IdentifierNameSyntax>(_ => _.Parent is ArgumentSyntax
                                                                              || (_.Parent is BinaryExpressionSyntax b && b.IsKind(SyntaxKind.CoalesceExpression) && b.Parent is ArgumentSyntax));

                return identifiers.ToHashSet(_ => _.Identifier.ValueText);
            }
        }
    }
}
