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
    public sealed class MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3100";

        private static readonly HashSet<string> PropertyNames = new HashSet<string>
            {
                "ObjectUnderTest",
                "Sut",
                "SuT",
                "SUT",
                "SubjectUnderTest",
                "UnitUnderTest",
                "Uut",
                "UuT",
                "UUT",
                "TestCandidate",
                "TestObject",
            };

        private static readonly string[] RawFieldNames =
            {
                "ObjectUnderTest",
                "objectUnderTest",
                "SubjectUnderTest",
                "subjectUnderTest",
                "Sut",
                "sut",
                "UnitUnderTest",
                "unitUnderTest",
                "Uut",
                "uut",
                "TestCandidate",
                "TestObject",
                "testCandidate",
                "testObject",
            };

        private static readonly HashSet<string> FieldNames = new[] { "", "_", "m_", "s_" }.SelectMany(_ => RawFieldNames, (prefix, name) => prefix + name).ToHashSet();

        private static readonly HashSet<string> VariableNames = new HashSet<string>
            {
                "objectUnderTest",
                "sut",
                "subjectUnderTest",
                "unitUnderTest",
                "uut",
                "testCandidate",
                "testObject",
            };

        private static readonly HashSet<string> MethodNames = new[] { "Create", "Get" }.SelectMany(_ => PropertyNames, (prefix, name) => prefix + name).ToHashSet();

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
                var members = symbol.GetMembers();
                var methodTypes = members.OfType<IMethodSymbol>().Where(_ => !_.ReturnsVoid).Where(_ => MethodNames.Contains(_.Name)).Select(_ => _.ReturnType);
                var propertyTypes = members.OfType<IPropertySymbol>().Where(_ => PropertyNames.Contains(_.Name)).Select(_ => _.GetReturnType());
                var fieldTypes = members.OfType<IFieldSymbol>().Where(_ => FieldNames.Contains(_.Name)).Select(_ => _.Type);
                var typesUnderTest = propertyTypes.Concat(fieldTypes).Concat(methodTypes);

                foreach (var typeUnderTest in typesUnderTest)
                {
                    if (TryAnalyzeType(symbol, typeUnderTest, out var diagnostic))
                        return new []{ diagnostic };
                }
            }

            return Enumerable.Empty<Diagnostic>();
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

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;
            var variable = node.Declaration;

            // inspect tests
            if (variable.Variables.Any(_ => VariableNames.Contains(_.Identifier.ValueText)))
            {
                // find method
                var semanticModel = context.SemanticModel;
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