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

        internal static readonly string[] MockNames = { "Mock", "mock", "Stub", "stub", "Fake", "fake", "Shim", "shim" };

        public MiKo_1108_MockNamingAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static string FindBetterName(ISymbol symbol)
        {
            var symbolName = symbol.Name.Without(MockNames);

            if (symbolName.IsNullOrWhiteSpace() || symbolName.StartsWithNumber())
            {
                return symbol.Name;
            }

            return symbolName;
        }

        protected override void InitializeCore(AnalysisContext context)
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
            if (ShallAnalyze(context))
            {
                base.AnalyzeDeclarationPattern(context);
            }
        }

        protected override void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                base.AnalyzeForEachStatement(context);
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => from syntaxToken in identifiers
                                                                                                                                        let name = syntaxToken.ValueText
                                                                                                                                        where name.Length > 3 && name.ContainsAny(MockNames)
                                                                                                                                        let symbol = syntaxToken.GetSymbol(semanticModel)
                                                                                                                                        select symbol is null
                                                                                                                                                   ? Issue(syntaxToken)
                                                                                                                                                   : Issue(symbol);

        private static bool ShallAnalyze(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();
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
            if (ShallAnalyze(context))
            {
                AnalyzeIdentifiers(context, (VariableDeclarationSyntax)context.Node);
            }
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                AnalyzeIdentifiers(context, ((PropertyDeclarationSyntax)context.Node).Identifier);
            }
        }

        private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                AnalyzeIdentifiers(context, ((FieldDeclarationSyntax)context.Node).Declaration);
            }
        }

        private void AnalyzeParameter(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                var syntax = (ParameterSyntax)context.Node;

                // ignore invocations eg. in lambdas
                if (syntax.GetEnclosing<InvocationExpressionSyntax>() is null)
                {
                    AnalyzeIdentifiers(context, syntax.Identifier);
                }
            }
        }

        private void AnalyzeIdentifiers(SyntaxNodeAnalysisContext context, VariableDeclarationSyntax syntax)
        {
            AnalyzeIdentifiers(context, syntax.Variables.Select(_ => _.Identifier).ToArray());
        }

        private void AnalyzeIdentifiers(SyntaxNodeAnalysisContext context, params SyntaxToken[] identifiers)
        {
            var diagnostics = AnalyzeIdentifiers(context.SemanticModel, identifiers);

            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}