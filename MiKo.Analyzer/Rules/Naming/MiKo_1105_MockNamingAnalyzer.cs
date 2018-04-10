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
    public sealed class MiKo_1105_MockNamingAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1105";

        public MiKo_1105_MockNamingAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeVariableDeclaration, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => true;

        private static bool ShallAnalyze(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();
            return type?.IsTestClass() == true;
        }

        protected override void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                base.AnalyzeDeclarationPattern(context);
            }
        }

        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                AnalyzeIdentifiers(context, ((VariableDeclarationSyntax)context.Node).Variables.Select(_ => _.Identifier).ToArray());
            }
        }

        private void AnalyzeParameter(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                var parameterSyntax = ((ParameterSyntax)context.Node);
                if (parameterSyntax.GetEnclosing<InvocationExpressionSyntax>() is null) // ignore invocations eg. in lambdas
                {
                    AnalyzeIdentifiers(context, parameterSyntax.Identifier);
                }
            }
        }

        private void AnalyzeIdentifiers(SyntaxNodeAnalysisContext context, params SyntaxToken[] identifiers)
        {
            var diagnostics = AnalyzeIdentifiers(context.SemanticModel, identifiers);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => identifiers
                                                                                                                                            .Where(_ => _.ValueText.ContainsAny(StringComparison.OrdinalIgnoreCase, "mock", "stub"))
                                                                                                                                            .Select(_ => ReportIssue(_.GetSymbol(semanticModel)));
    }
}