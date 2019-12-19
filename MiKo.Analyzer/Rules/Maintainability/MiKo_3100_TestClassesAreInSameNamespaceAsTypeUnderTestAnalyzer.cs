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

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);

        private static IEnumerable<ITypeSymbol> GetTypeUnderTestTypes(ITypeSymbol testClass, SemanticModel semanticModel)
        {
            var typesUnderTest = new HashSet<ITypeSymbol>();

            // Idea:
            // 1. Collect created types of objects (ObjectCreationExpression) that are either directly returned (or assigned to variable that is returned)
            foreach (var methodSymbol in testClass.GetMembers().OfType<IMethodSymbol>().Where(_ => _.IsTypeUnderTestCreationMethod()))
            {
                var methodDeclaration = (MethodDeclarationSyntax)methodSymbol.GetSyntax();

                var types = AnalyzeTestCreationMethod(methodDeclaration, semanticModel);
                foreach (var type in types)
                {
                    typesUnderTest.Add(type);
                }
            }

            // if anything is found, we only use those as those are the most concrete ones
            if (typesUnderTest.Any())
            {
                return typesUnderTest;
            }

            // 2. If none is found, go into each test method and try to find out which objects get created that are assigned to a local variable named 'objectUnderTest' (or similar)
            foreach (var methodSymbol in testClass.GetMembers().OfType<IMethodSymbol>().Where(_ => _.IsTestSetupMethod() || _.IsTestMethod()))
            {
                var methodDeclaration = (MethodDeclarationSyntax)methodSymbol.GetSyntax();

                var types = AnalyzeTestMethod(methodDeclaration, semanticModel);
                foreach (var type in types)
                {
                    typesUnderTest.Add(type);
                }
            }

            // if anything is found, we only use those as those are the most concrete ones
            if (typesUnderTest.Any())
            {
                return typesUnderTest;
            }

            // 3. If none is found, check for fields or properties that are named ObjectUnderTest
            return testClass.GetTypeUnderTestTypes();
        }

        private static IEnumerable<ITypeSymbol> AnalyzeTestCreationMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
        {
            var types = new HashSet<ITypeSymbol>();

            if (methodDeclaration.Body is null)
            {
                if (methodDeclaration.ExpressionBody?.Expression is ObjectCreationExpressionSyntax oces)
                {
                    var typeInfo = semanticModel.GetTypeInfo(oces);
                    var type = typeInfo.Type;

                    types.Add(type);
                }
            }
            else
            {
                var controlFlow = semanticModel.AnalyzeControlFlow(methodDeclaration.Body);
                var returnStatements = controlFlow.ReturnStatements.OfType<ReturnStatementSyntax>();

                foreach (var variable in methodDeclaration.DescendantNodes().OfType<VariableDeclarationSyntax>().SelectMany(_ => _.Variables))
                {
                    if (returnStatements.Any(_ => _.Expression is IdentifierNameSyntax ins && variable.Identifier.Text == ins.Identifier.Text) && variable.Initializer.Value is ObjectCreationExpressionSyntax oces)
                    {
                        var typeInfo = semanticModel.GetTypeInfo(oces);
                        var type = typeInfo.Type;

                        types.Add(type);
                    }
                }
            }

            return types;
        }

        private static IEnumerable<ITypeSymbol> AnalyzeTestMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
        {
            var types = new HashSet<ITypeSymbol>();

            foreach (var variableDeclaration in methodDeclaration.DescendantNodes().OfType<VariableDeclarationSyntax>())
            {
                // inspect associated test method
                if (variableDeclaration.Variables.Any(_ => _.IsTypeUnderTestVariable()))
                {
                    var typeUnderTest = variableDeclaration.GetTypeSymbol(semanticModel);
                    types.Add(typeUnderTest);
                }
            }

            return types;
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var semanticModel = context.SemanticModel;

            var node = (ClassDeclarationSyntax)context.Node;

            var testClass = node.GetTypeSymbol(semanticModel);

            if (testClass.IsTestClass())
            {
                var typesUnderTest = GetTypeUnderTestTypes(testClass, semanticModel);

                foreach (var typeUnderTest in typesUnderTest)
                {
                    if (TypeHasNamespaceIssue(testClass, typeUnderTest, out var diagnostic))
                    {
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool TypeHasNamespaceIssue(ITypeSymbol testClass, ITypeSymbol typeUnderTest, out Diagnostic result)
        {
            if (typeUnderTest != null)
            {
                var expectedNamespace = typeUnderTest.ContainingNamespace.FullyQualifiedName();
                var unitTestNamespace = testClass.ContainingNamespace.FullyQualifiedName();

                if (expectedNamespace != unitTestNamespace)
                {
                    result = Issue(testClass, expectedNamespace);
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}