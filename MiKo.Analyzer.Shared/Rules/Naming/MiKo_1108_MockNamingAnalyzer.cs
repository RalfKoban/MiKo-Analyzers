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

        private static readonly string[] MockNames = { "Mocked", "mocked", "Mock", "mock", "Stub", "stub", "Faked", "Fake", "faked", "fake", "Shim", "shim" };

        public MiKo_1108_MockNamingAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

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

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;

                if (name.Length < 4)
                {
                    continue;
                }

                if (name.ContainsAny(MockNames))
                {
                    var symbol = identifier.GetSymbol(semanticModel);

                    if (symbol is null)
                    {
                        yield return Issue(identifier);
                    }
                    else
                    {
                        var betterName = FindBetterName(symbol);

                        yield return Issue(symbol, CreateBetterNameProposal(betterName));
                    }
                }
            }
        }

        private static string FindBetterName(ISymbol symbol)
        {
            var symbolName = symbol.Name;
            var betterName = symbolName.Without(MockNames);

            if (betterName.IsNullOrWhiteSpace() || betterName.StartsWithNumber())
            {
                return symbolName;
            }

            if (symbol is ILocalSymbol || symbol is IParameterSymbol)
            {
                if (symbolName[0].IsLowerCaseLetter())
                {
                    return betterName.ToLowerCaseAt(0);
                }
            }

            return betterName;
        }

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
                var variableDeclarationSyntax = (VariableDeclarationSyntax)context.Node;

                if (variableDeclarationSyntax.Parent is FieldDeclarationSyntax)
                {
                    // nothing to do, we already analyzed the field separately
                }
                else
                {
                    AnalyzeIdentifiers(context, type, variableDeclarationSyntax);
                }
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

                // ignore invocations e.g. in lambdas
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