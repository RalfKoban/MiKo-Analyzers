using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1100_TestClassesPrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1100";

        public MiKo_1100_TestClassesPrefixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            base.InitializeCore(context);

            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var typesUnderTest = symbol.GetTypeUnderTestTypes().ToList();
            if (typesUnderTest.Any())
            {
                var typeUnderTestNames = typesUnderTest.Select(_ => GetTypeUnderTestName(symbol, _))
                                                       .Where(_ => _ != null) // ignore generic class or struct constraint
                                                       .ToList();

                if (typeUnderTestNames.Any(_ => TestClassStartsWithName(symbol, _)))
                {
                    // names seem to match
                    return Enumerable.Empty<Diagnostic>();
                }

                if (typeUnderTestNames.Any())
                {
                    // non-matching types, maybe we have some base types and have to investigate the creation methods
                    return TestClassIsNamedAfterCreatedTypeUnderTest(symbol)
                               ? Enumerable.Empty<Diagnostic>()
                               : new[] { Issue(symbol, typeUnderTestNames.First() + Constants.TestsSuffix) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static string GetTypeUnderTestName(INamedTypeSymbol testClass, ITypeSymbol typeUnderTest)
        {
            if (typeUnderTest.TypeKind != TypeKind.TypeParameter)
            {
                return typeUnderTest.Name;
            }

            var typeParameter = (ITypeParameterSymbol)testClass.TypeArguments[0];

            // for generic class or struct constraints there is no constraint type available
            var constraint = typeParameter.ConstraintTypes.FirstOrDefault();

            return constraint?.Name;
        }

        private static bool TestClassStartsWithName(ITypeSymbol testClass, string typeUnderTestName) => testClass.Name.StartsWith(typeUnderTestName, StringComparison.Ordinal);

        private static bool TestClassIsNamedAfterCreatedTypeUnderTest(ITypeSymbol testClass)
        {
            var types = testClass.GetCreatedObjectSyntaxReturnedByMethods().Select(_ => _.Type);

            foreach (var type in types)
            {
                var typeName = type.GetNameOnlyPart();
                if (typeName is null)
                {
                    continue;
                }

                if (TestClassStartsWithName(testClass, typeName))
                {
                    return true;
                }
            }

            return false;
        }

        private new void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
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
                    var typeUnderTestName = GetTypeUnderTestName(testClass, typeUnderTest);

                    if (TestClassStartsWithName(testClass, typeUnderTestName) is false)
                    {
                        var issue = Issue(testClass, typeUnderTestName + Constants.TestsSuffix);

                        ReportDiagnostics(context, issue);
                    }
                }
            }
        }
    }
}