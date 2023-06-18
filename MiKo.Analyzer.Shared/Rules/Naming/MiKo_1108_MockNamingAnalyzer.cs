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
    public sealed class MiKo_1108_MockNamingAnalyzer : NamingAnalyzer // NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1108";

        internal static readonly string[] MockNames = { "Mocked", "mocked", "Mock", "mock", "Stub", "stub", "Faked", "Fake", "faked", "fake", "Shim", "shim" };

        public MiKo_1108_MockNamingAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(ISymbol symbol)
        {
            var symbolName = symbol.Name.Without(MockNames);

            if (symbolName.IsNullOrWhiteSpace() || symbolName.StartsWithNumber())
            {
                return symbol.Name;
            }

            if (symbol is ILocalSymbol || symbol is IParameterSymbol)
            {
                if (symbol.Name[0].IsLowerCaseLetter())
                {
                    return symbolName.ToLowerCaseAt(0);
                }
            }

            return symbolName;
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);

            context.RegisterSyntaxNodeAction(AnalyzeVariableDeclaration, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
        }

        protected override void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();

            if (ShallAnalyze(type))
            {
                base.AnalyzeDeclarationPattern(context);
            }
        }

        protected override void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();

            if (ShallAnalyze(type))
            {
                base.AnalyzeForEachStatement(context);
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => from identifier in identifiers
                                                                                                                                                          let name = identifier.ValueText
                                                                                                                                                          where name.Length > 3 && name.ContainsAny(MockNames)
                                                                                                                                                          let symbol = identifier.GetSymbol(semanticModel)
                                                                                                                                                          select symbol is null
                                                                                                                                                                 ? Issue(identifier)
                                                                                                                                                                 : Issue(symbol);

        private static bool ShallAnalyze(INamedTypeSymbol type)
        {
            if (type is null)
            {
                return false;
            }

            if (type.IsTestClass())
            {
                return true;
            }

            var assemblyName = type.ContainingAssembly.Name;

            if (assemblyName.Contains("Test"))
            {
                return assemblyName.Contains("MiKoSolutions.Analyzers") is false;
            }

            return false;
        }

        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();

            if (ShallAnalyze(type))
            {
                AnalyzeIdentifiers(context, type, (VariableDeclarationSyntax)context.Node);
            }
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();

            if (ShallAnalyze(type))
            {
                AnalyzeIdentifiers(context, type, ((PropertyDeclarationSyntax)context.Node).Identifier);
            }
        }

        private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();

            if (ShallAnalyze(type))
            {
                AnalyzeIdentifiers(context, type, ((FieldDeclarationSyntax)context.Node).Declaration);
            }
        }

        private void AnalyzeParameter(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();

            if (ShallAnalyze(type))
            {
                var syntax = (ParameterSyntax)context.Node;

                // ignore invocations eg. in lambdas
                if (syntax.GetEnclosing<InvocationExpressionSyntax>() is null)
                {
                    AnalyzeIdentifiers(context, type, syntax.Identifier);
                }
            }
        }

        private void AnalyzeIdentifiers(SyntaxNodeAnalysisContext context, ITypeSymbol type, VariableDeclarationSyntax syntax)
        {
            AnalyzeIdentifiers(context, type, syntax.Variables.Select(_ => _.Identifier).ToArray());
        }

        private void AnalyzeIdentifiers(SyntaxNodeAnalysisContext context, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            var issues = AnalyzeIdentifiers(context.SemanticModel, type, identifiers);

            ReportDiagnostics(context, issues);
        }
    }
}