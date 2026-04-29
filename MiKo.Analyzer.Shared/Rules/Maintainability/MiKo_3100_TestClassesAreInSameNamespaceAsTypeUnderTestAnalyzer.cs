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
    public sealed class MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3100";

        public MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);

        private static IReadOnlyCollection<ITypeSymbol> GetTypeUnderTestTypes(ITypeSymbol testClass, SemanticModel semanticModel)
        {
            var syntaxTree = semanticModel.SyntaxTree;

            var allMethods = testClass.GetNamedMethods();
            var methods = allMethods.Where(_ => _.GetSyntax()?.SyntaxTree == syntaxTree).ToList();

            if (allMethods.Count != methods.Count && testClass.IsPartial())
            {
                // different syntax trees means that it's inside a partial part which we do not want/need to inspect (because namespaces for such parts are all the same and we only need to inspect one part)
                return Array.Empty<ITypeSymbol>();
            }

            var typesUnderTest = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

            // Idea:
            // 1. Collect created types of objects (ObjectCreationExpression) that are either directly returned (or assigned to variable that is returned)
            foreach (var method in methods.Where(_ => _.IsTypeUnderTestCreationMethod()))
            {
                var methodDeclaration = (MethodDeclarationSyntax)method.GetSyntax();

                var type = AnalyzeTestCreationMethod(methodDeclaration, semanticModel);

                if (type != null)
                {
                    typesUnderTest.Add(type);
                }
            }

            // if anything is found, we only use those as those are the most concrete ones
            if (typesUnderTest.Count != 0)
            {
                return typesUnderTest;
            }

            // 2. If none is found, go into each test method and try to find out which objects get created that are assigned to a local variable named 'objectUnderTest' (or similar)
            foreach (var method in methods.Where(_ => _.IsTestSetUpMethod() || _.IsTestMethod()))
            {
                var methodDeclaration = (MethodDeclarationSyntax)method.GetSyntax();

                var type = AnalyzeTestMethod(methodDeclaration, semanticModel);

                if (type != null)
                {
                    typesUnderTest.Add(type);
                }
            }

            // if anything is found, we only use those as those are the most concrete ones
            if (typesUnderTest.Count != 0)
            {
                return typesUnderTest;
            }

            // 3. If none is found, check for fields or properties that are named ObjectUnderTest
            return testClass.GetTypeUnderTestTypes();
        }

        private static ITypeSymbol AnalyzeTestCreationMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
        {
            var body = methodDeclaration.Body;

            if (body is null)
            {
                if (methodDeclaration.ExpressionBody?.Expression is ObjectCreationExpressionSyntax oces)
                {
                    var typeUnderTest = oces.GetTypeSymbol(semanticModel);

                    return typeUnderTest;
                }
            }
            else
            {
                IEnumerable<ReturnStatementSyntax> returnStatements = null;

                foreach (var variable in methodDeclaration.DescendantNodes<VariableDeclarationSyntax>(SyntaxKind.VariableDeclaration).SelectMany(_ => _.Variables))
                {
                    if (variable.Initializer?.Value is ObjectCreationExpressionSyntax oces)
                    {
                        if (returnStatements is null)
                        {
                            var controlFlow = semanticModel.AnalyzeControlFlow(body);
                            var statements = controlFlow.ReturnStatements;

                            if (statements.Length is 0)
                            {
                                // No return statements in the method body at all, so no variable can be returned
                                return null;
                            }

                            returnStatements = statements.OfType<ReturnStatementSyntax>();
                        }

                        var variableName = variable.GetName();

                        if (returnStatements.Any(_ => _.Expression is IdentifierNameSyntax ins && variableName == ins.GetName()))
                        {
                            var typeUnderTest = oces.GetTypeSymbol(semanticModel);

                            return typeUnderTest;
                        }
                    }
                }
            }

            return null;
        }

        private static ITypeSymbol AnalyzeTestMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
        {
            if (methodDeclaration.Body is var body)
            {
                foreach (var variableDeclaration in body.DescendantNodes<VariableDeclarationSyntax>(SyntaxKind.VariableDeclaration))
                {
                    // inspect associated test method
                    if (variableDeclaration.Variables.Any(_ => _.IsTypeUnderTestVariable()))
                    {
                        var typeUnderTest = variableDeclaration.GetTypeSymbol(semanticModel);

                        return typeUnderTest;
                    }
                }
            }

            return null;
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var issues = AnalyzeTypeSymbol(context);

            ReportDiagnostics(context, issues);
        }

        private Diagnostic[] AnalyzeTypeSymbol(SyntaxNodeAnalysisContext context)
        {
            if (context.FindContainingType() is ITypeSymbol testClass && testClass.IsTestClass())
            {
                var typesUnderTest = GetTypeUnderTestTypes(testClass, context.SemanticModel);

                if (typesUnderTest.Count > 0)
                {
                    List<Diagnostic> issues = null;

                    foreach (var typeUnderTest in typesUnderTest)
                    {
                        var issue = TypeHasNamespaceIssue(testClass, typeUnderTest);

                        if (issue is null)
                        {
                            continue;
                        }

                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(issue);
                    }

                    if (issues != null)
                    {
                        return issues.ToArray();
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic TypeHasNamespaceIssue(ITypeSymbol testClass, ITypeSymbol typeUnderTest)
        {
            if (typeUnderTest != null)
            {
                var expectedNamespace = typeUnderTest.ContainingNamespace.FullyQualifiedName();
                var unitTestNamespace = testClass.ContainingNamespace.FullyQualifiedName();

                if (expectedNamespace != unitTestNamespace)
                {
                    return Issue(testClass, expectedNamespace);
                }
            }

            return null;
        }
    }
}