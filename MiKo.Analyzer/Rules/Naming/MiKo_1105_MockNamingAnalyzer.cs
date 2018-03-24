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
        }

        protected override void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                base.AnalyzeDeclarationPattern(context);
            }
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => true;

        private static bool ShallAnalyze(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();
            return type?.IsTestClass() == true;
        }

        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                AnalyzeVariableDeclaration(context, ((VariableDeclarationSyntax)context.Node).Variables.Select(_ => _.Identifier).ToArray());
            }
        }

        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context, params SyntaxToken[] identifiers)
        {
            var diagnostics = Analyze(context.SemanticModel, identifiers);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        protected override IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> results = null;

            foreach (var identifier in identifiers)
            {
                var variableName = identifier.ValueText;

                if (!variableName.ContainsAny(StringComparison.OrdinalIgnoreCase, "mock", "stub")) continue;

                var symbol = semanticModel.LookupSymbols(identifier.GetLocation().SourceSpan.Start, name: variableName).First();

                if (results == null) results = new List<Diagnostic>();
                results.Add(ReportIssue(symbol));
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}