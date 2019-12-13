﻿using System.Collections.Generic;
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

        private static bool TestClassHasNamespaceOfTypeUnderTestCreatedInCode(ITypeSymbol testClass, IEnumerable<TypeSyntax> otherTypesUnderTest, SemanticModel semanticModel)
        {
            var unitTestNamespace = testClass.ContainingNamespace.FullyQualifiedName();

            var namespacesMatch = otherTypesUnderTest.Select(_ => _.GetTypeSymbol(semanticModel))
                                                     .Select(_ => _.ContainingNamespace.FullyQualifiedName())
                                                     .Any(_ => _ == unitTestNamespace);
            return namespacesMatch;
        }

        private bool TypeHasNamespaceIssue(ITypeSymbol testClass, ITypeSymbol typeUnderTest, IEnumerable<TypeSyntax> otherTypesUnderTest, SemanticModel semanticModel, out Diagnostic result)
        {
            if (typeUnderTest != null)
            {
                var expectedNamespace = typeUnderTest.ContainingNamespace.FullyQualifiedName();
                var unitTestNamespace = testClass.ContainingNamespace.FullyQualifiedName();

                if (expectedNamespace != unitTestNamespace)
                {
                    // try to find an assignment to see if a more concrete type is a better match
                    if (TestClassHasNamespaceOfTypeUnderTestCreatedInCode(testClass, otherTypesUnderTest, semanticModel) is false)
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

            var symbol = node.GetTypeSymbol(context.SemanticModel);

            if (symbol.IsTestClass())
            {
                var typesUnderTest = symbol.GetTypeUnderTestTypes();

                var otherTypesUnderTest = symbol.GetTypeUnderTestTypeSyntaxesCreatedInCode().ToList();

                foreach (var typeUnderTest in typesUnderTest)
                {
                    if (TypeHasNamespaceIssue(symbol, typeUnderTest, otherTypesUnderTest, context.SemanticModel, out var diagnostic))
                    {
                        context.ReportDiagnostic(diagnostic);
                    }
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
                    var otherTypesUnderTest = testClass.GetTypeUnderTestTypeSyntaxesCreatedInCode();

                    var typeUnderTest = declaration.GetTypeSymbol(context.SemanticModel);

                    if (TypeHasNamespaceIssue(testClass, typeUnderTest, otherTypesUnderTest, context.SemanticModel, out var diagnostic))
                    {
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}