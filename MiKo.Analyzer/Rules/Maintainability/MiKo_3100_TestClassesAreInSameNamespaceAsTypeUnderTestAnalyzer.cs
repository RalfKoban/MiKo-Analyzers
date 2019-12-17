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

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        private static Lazy<IEnumerable<ITypeSymbol>> GetTypeUnderTestTypeSyntaxesCreatedInCode(ITypeSymbol symbol, SemanticModel semanticModel)
        {
            return new Lazy<IEnumerable<ITypeSymbol>>(() => symbol.GetCreatedObjectSyntaxReturnedByMethod()
                                                                  .Select(_ => semanticModel.GetTypeInfo(_))
                                                                  .Select(_ => _.Type)
                                                                  .ToList());
        }

        private static bool NamespaceMatchesTypeUnderTestCreatedInCode(ITypeSymbol testClass, Lazy<IEnumerable<ITypeSymbol>> otherTypesUnderTest)
        {
            var unitTestNamespace = testClass.ContainingNamespace.FullyQualifiedName();

            var namespacesMatch = otherTypesUnderTest.Value
                                                     .Select(_ => _.ContainingNamespace.FullyQualifiedName())
                                                     .Any(_ => _ == unitTestNamespace);
            return namespacesMatch;
        }

        private bool TypeHasNamespaceIssue(ITypeSymbol testClass, ITypeSymbol typeUnderTest, Lazy<IEnumerable<ITypeSymbol>> otherTypesUnderTest, out Diagnostic result)
        {
            if (typeUnderTest != null)
            {
                var expectedNamespace = typeUnderTest.ContainingNamespace.FullyQualifiedName();
                var unitTestNamespace = testClass.ContainingNamespace.FullyQualifiedName();

                if (expectedNamespace != unitTestNamespace)
                {
                    // try to find an assignment to see if a more concrete type is a better match
                    if (NamespaceMatchesTypeUnderTestCreatedInCode(testClass, otherTypesUnderTest) is false)
                    {
                        result = Issue(testClass, expectedNamespace);
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (ClassDeclarationSyntax)context.Node;

            var typeSymbol = node.GetTypeSymbol(context.SemanticModel);

            if (typeSymbol.IsTestClass())
            {
                var typesUnderTest = typeSymbol.GetTypeUnderTestTypes();
                var otherTypesUnderTest = GetTypeUnderTestTypeSyntaxesCreatedInCode(typeSymbol, context.SemanticModel);

                foreach (var typeUnderTest in typesUnderTest)
                {
                    AnalyzeNamespaces(context, typeSymbol, typeUnderTest, otherTypesUnderTest);
                }
            }
        }

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;
            var declaration = node.Declaration;

            if (declaration.Variables.Any(_ => _.IsTypeUnderTestVariable()))
            {
                // inspect associated test method
                var method = context.GetEnclosingMethod();
                if (method.IsTestMethod())
                {
                    var testClass = method.ContainingType;
                    var typeUnderTest = declaration.GetTypeSymbol(context.SemanticModel);
                    var otherTypesUnderTest = GetTypeUnderTestTypeSyntaxesCreatedInCode(testClass, context.SemanticModel);

                    AnalyzeNamespaces(context, testClass, typeUnderTest, otherTypesUnderTest);
                }
            }
        }

        private void AnalyzeNamespaces(SyntaxNodeAnalysisContext context, ITypeSymbol testClass, ITypeSymbol typeUnderTest, Lazy<IEnumerable<ITypeSymbol>> otherTypesUnderTest)
        {
            if (TypeHasNamespaceIssue(testClass, typeUnderTest, otherTypesUnderTest, out var diagnostic))
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}