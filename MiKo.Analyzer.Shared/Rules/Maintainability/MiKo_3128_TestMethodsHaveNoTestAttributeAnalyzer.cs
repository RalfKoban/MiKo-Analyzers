using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3128_TestMethodsHaveNoTestAttributeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3128";

        public MiKo_3128_TestMethodsHaveNoTestAttributeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(INamedTypeSymbol symbol)
        {
            if (symbol.IsPubliclyVisible())
            {
                var symbolName = symbol.Name.AsSpan();

                return symbolName.EndsWith("Assert") is false
                    && symbolName.EndsWith("Constraint") is false;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            foreach (var method in symbol.GetMethods(MethodKind.Ordinary))
            {
                if (method.DeclaredAccessibility is Accessibility.Public && method.IsTestMethod() is false)
                {
                    if (method.GetSyntax<BaseMethodDeclarationSyntax>().DescendantNodes<ExpressionStatementSyntax>().Any(IsAssert))
                    {
                        // we should have a test
                        var attributeName = GetAttributeName(compilation);

                        if (attributeName != null)
                        {
                            yield return Issue(method, new Pair(Constants.AnalyzerCodeFixSharedData.Marker, attributeName));
                        }
                    }
                }
            }
        }

        private static string GetAttributeName(Compilation compilation)
        {
            if (ReferencesNUnit(compilation))
            {
                return Constants.Names.TestAttribute;
            }

            if (ReferencesXUnit(compilation))
            {
                return Constants.Names.FactAttribute;
            }

            if (ReferencesMsTest(compilation))
            {
                return Constants.Names.TestMethodAttribute;
            }

            return null;
        }

        private static bool IsAssert(ExpressionStatementSyntax statement) => statement.Expression is InvocationExpressionSyntax i && i.GetIdentifierName().EndsWith("Assert", StringComparison.Ordinal);
    }
}