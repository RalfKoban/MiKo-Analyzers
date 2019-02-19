using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                "Sut",
                "SUT",
                "SubjectUnderTest",
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

        private static readonly string[] VariableNames =
            {
                "objectUnderTest",
                "sut",
                "subjectUnderTest",
            };

        public MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer() : base(Id, SymbolKind.NamedType)
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
                foreach (var typeUnderTest in PropertyNames.Select(_ => symbol.GetMembers(_).OfType<IPropertySymbol>().FirstOrDefault()?.GetReturnType()))
                {
                    if (TryAnalyzeType(symbol, typeUnderTest, out var diagnostic))
                        return new []{ diagnostic };
                }

                foreach (var typeUnderTest in FieldNames.Select(_ => symbol.GetMembers(_).OfType<IFieldSymbol>().FirstOrDefault()?.Type))
                {
                    if (TryAnalyzeType(symbol, typeUnderTest, out var diagnostic))
                        return new[] { diagnostic };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private bool TryAnalyzeType(INamedTypeSymbol symbol, ITypeSymbol typeUnderTest, out Diagnostic diagnostic)
        {
            if (typeUnderTest != null)
            {
                var expectedNamespace = typeUnderTest.ContainingNamespace.ToDisplayString();
                var unitTestNamespace = symbol.ContainingNamespace.ToDisplayString();

                if (expectedNamespace != unitTestNamespace)
                {
                    diagnostic = ReportIssue(symbol, expectedNamespace);
                    return true;
                }
            }

            diagnostic = null;
            return false;
        }

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            var variable = node.Declaration;

            // inspect tests
            foreach (var variableName in VariableNames)
            {
                if (variable.Variables.Any(_ => _.Identifier.ValueText == variableName))
                {
                    // find method
                    var method = node.GetEnclosingMethod(semanticModel);
                    if (method.IsTestMethod())
                    {
                        var typeUnderTest = semanticModel.GetTypeInfo(variable.Type).Type;
                        if (TryAnalyzeType(method.ContainingType, typeUnderTest, out var diagnostic))
                        {
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }
    }
}