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

        private static readonly HashSet<string> TypeUnderTestVariableNames = new HashSet<string>
            {
                "objectUnderTest",
                "sut",
                "subjectUnderTest",
                "unitUnderTest",
                "uut",
                "testCandidate",
                "testObject",
            };

        public MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            base.InitializeCore(context);

            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            if (symbol.IsTestClass())
            {
                var typesUnderTest = symbol.GetTypeUnderTestTypes();
                foreach (var typeUnderTest in typesUnderTest)
                {
                    if (TryAnalyzeType(symbol, typeUnderTest, out var diagnostic))
                        return new []{ diagnostic };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static bool IsTypeUnderTestVariable(VariableDeclarationSyntax variable) => variable.Variables.Any(_ => TypeUnderTestVariableNames.Contains(_.Identifier.ValueText));

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;
            var variable = node.Declaration;

            if (IsTypeUnderTestVariable(variable))
            {
                // inspect associated test method
                var method = context.GetEnclosingMethod();
                if (method.IsTestMethod())
                {
                    var typeUnderTest = variable.GetTypeSymbol(context.SemanticModel);

                    if (TryAnalyzeType(method.ContainingType, typeUnderTest, out var diagnostic))
                    {
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool TryAnalyzeType(INamedTypeSymbol symbol, ITypeSymbol typeUnderTest, out Diagnostic diagnostic)
        {
            if (typeUnderTest != null)
            {
                var expectedNamespace = typeUnderTest.ContainingNamespace.FullyQualifiedName();
                var unitTestNamespace = symbol.ContainingNamespace.FullyQualifiedName();

                if (expectedNamespace != unitTestNamespace)
                {
                    diagnostic = Issue(symbol, expectedNamespace);
                    return true;
                }
            }

            diagnostic = null;
            return false;
        }
    }
}