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
    public sealed class MiKo_1106_MockNamingAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1106";

        public MiKo_1106_MockNamingAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeVariableDeclaration, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        }

        protected override void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                base.AnalyzeDeclarationPattern(context);
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var syntaxToken in identifiers.Where(_ => _.ValueText.ContainsAny("mock", "stub")))
            {
                var symbol = syntaxToken.GetSymbol(semanticModel);
                var diagnostic = symbol != null
                                     ? ReportIssue(symbol)
                                     : ReportIssue(syntaxToken.ValueText, syntaxToken.GetLocation());
                yield return diagnostic;
            }
        }

        private static bool ShallAnalyze(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();
            if (type is null)
                return false;

            if (type.IsTestClass())
                return true;

            var assemblyName = type.ContainingAssembly.Name;
            if (assemblyName.Contains("Test"))
                return !assemblyName.Contains("MiKoSolutions.Analyzers");

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
                if (syntax.GetEnclosing<InvocationExpressionSyntax>() is null) // ignore invocations eg. in lambdas
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