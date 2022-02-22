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

        private static IEnumerable<ITypeSymbol> GetTypeUnderTestTypes(ITypeSymbol testClass, SemanticModel semanticModel)
        {
            var syntaxTree = semanticModel.SyntaxTree;

            var allMethods = testClass.GetNamedMethods().ToList();
            var methods = allMethods.Where(_ => _.GetSyntax()?.SyntaxTree == syntaxTree).ToList();

            if (testClass.IsPartial() && allMethods.Count != methods.Count)
            {
                // different syntax trees means that it's inside a partial part which we do not want/need to inspect (because namespaces for such parts are all the same and we only need to inspect one part)
                return Enumerable.Empty<ITypeSymbol>();
            }

            var typesUnderTest = new HashSet<ITypeSymbol>();

            // Idea:
            // 1. Collect created types of objects (ObjectCreationExpression) that are either directly returned (or assigned to variable that is returned)
            foreach (var method in methods.Where(_ => _.IsTypeUnderTestCreationMethod()))
            {
                var methodDeclaration = (MethodDeclarationSyntax)method.GetSyntax();

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
            foreach (var method in methods.Where(_ => _.IsTestSetUpMethod() || _.IsTestMethod()))
            {
                var methodDeclaration = (MethodDeclarationSyntax)method.GetSyntax();

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
            return testClass.GetTypeUnderTestTypes().ToList();
        }

        private static IEnumerable<ITypeSymbol> AnalyzeTestCreationMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
        {
            var types = new HashSet<ITypeSymbol>();

            if (methodDeclaration.Body is null)
            {
                if (methodDeclaration.ExpressionBody?.Expression is ObjectCreationExpressionSyntax oces)
                {
                    var type = oces.GetTypeSymbol(semanticModel);
                    types.Add(type);
                }
            }
            else
            {
                var controlFlow = semanticModel.AnalyzeControlFlow(methodDeclaration.Body);
                var returnStatements = controlFlow.ReturnStatements.OfType<ReturnStatementSyntax>();

                foreach (var variable in methodDeclaration.DescendantNodes<VariableDeclarationSyntax>().SelectMany(_ => _.Variables))
                {
                    if (returnStatements.Any(_ => _.Expression is IdentifierNameSyntax ins && variable.GetName() == ins.GetName()) && variable.Initializer?.Value is ObjectCreationExpressionSyntax oces)
                    {
                        var type = oces.GetTypeSymbol(semanticModel);
                        types.Add(type);
                    }
                }
            }

            return types;
        }

        private static IEnumerable<ITypeSymbol> AnalyzeTestMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
        {
            var types = new HashSet<ITypeSymbol>();

            foreach (var variableDeclaration in methodDeclaration.DescendantNodes<VariableDeclarationSyntax>())
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
            var node = (ClassDeclarationSyntax)context.Node;

            var issues = AnalyzeClassDeclaration(context, node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax node)
        {
            var semanticModel = context.SemanticModel;

            var testClass = node.GetTypeSymbol(semanticModel);

            if (testClass.IsTestClass())
            {
                var typesUnderTest = GetTypeUnderTestTypes(testClass, semanticModel);

                foreach (var typeUnderTest in typesUnderTest)
                {
                    yield return TypeHasNamespaceIssue(testClass, typeUnderTest);
                }
            }
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