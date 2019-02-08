using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3100";

        private static readonly string[] PropertyNames =
            {
                "ObjectUnderTest",
                "SubjectUnderTest",
                "Sut",
                "SUT",
            };

        private static readonly string[] FieldNames =
            {
                "ObjectUnderTest",
                "_ObjectUnderTest",
                "m_ObjectUnderTest",
                "s_ObjectUnderTest",

                "objectUnderTest",
                "_objectUnderTest",
                "m_objectUnderTest",
                "s_objectUnderTest",

                "SubjectUnderTest",
                "_SubjectUnderTest",
                "m_SubjectUnderTest",
                "s_SubjectUnderTest",

                "subjectUnderTest",
                "_subjectUnderTest",
                "m_subjectUnderTest",
                "s_subjectUnderTest",

                "Sut",
                "_Sut",
                "m_Sut",
                "s_Sut",

                "sut",
                "_sut",
                "m_sut",
                "s_sut",
            };

        public MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            if (symbol.IsTestClass())
            {
                foreach (var propertyName in PropertyNames)
                {
                    var typeUnderTest = symbol.GetMembers(propertyName).OfType<IPropertySymbol>().FirstOrDefault()?.GetReturnType();

                    if (TryAnalyzeType(symbol, typeUnderTest, out var diagnostics))
                        return diagnostics;
                }

                foreach (var fieldName in FieldNames)
                {
                    var typeUnderTest = symbol.GetMembers(fieldName).OfType<IFieldSymbol>().FirstOrDefault()?.Type;

                    if (TryAnalyzeType(symbol, typeUnderTest, out var diagnostics))
                        return diagnostics;

                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private bool TryAnalyzeType(INamedTypeSymbol symbol, ITypeSymbol typeUnderTest, out IEnumerable<Diagnostic> diagnostics)
        {
            if (typeUnderTest != null)
            {
                var testNamespace = symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var typeUnderTestNamespace = typeUnderTest.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (testNamespace != typeUnderTestNamespace)
                {
                    diagnostics = new[] { ReportIssue(symbol) };
                    return true;
                }
            }

            diagnostics = null;
            return false;
        }
    }
}