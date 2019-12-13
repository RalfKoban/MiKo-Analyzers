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

        public MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            base.InitializeCore(context);

            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsTestClass();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol)
        {
            var typesUnderTest = symbol.GetTypeUnderTestTypes();
            foreach (var typeUnderTest in typesUnderTest)
            {
                if (TryAnalyzeType(symbol, typeUnderTest, out var diagnostic))
                {
                    return new[] { diagnostic };
                }
            }

            return Enumerable.Empty<Diagnostic>();
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
                    var typeUnderTest = declaration.GetTypeSymbol(context.SemanticModel);

                    if (TryAnalyzeType(method.ContainingType, typeUnderTest, out var diagnostic))
                    {
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool TryAnalyzeType(INamedTypeSymbol symbol, ITypeSymbol typeUnderTest, out Diagnostic result)
        {
            if (typeUnderTest != null)
            {
                var expectedNamespace = typeUnderTest.ContainingNamespace.FullyQualifiedName();
                var unitTestNamespace = symbol.ContainingNamespace.FullyQualifiedName();

                if (expectedNamespace != unitTestNamespace)
                {
                    // TODO: try to find assignment to see if a more concrete type is better match
                    result = Issue(symbol, expectedNamespace);
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}