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

        private static bool ShallAnalyze(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();
            return type != null && type.IsTestClass();
        }

        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                AnalyzeVariableDeclaration(context, ((VariableDeclarationSyntax)context.Node).Variables.Select(_ => _.Identifier).ToArray());
            }
        }

        private void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            if (ShallAnalyze(context))
            {
                var node = (DeclarationPatternSyntax)context.Node;

                var diagnostics = Analyze(context.SemanticModel, node.Designation);
                foreach (var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, VariableDesignationSyntax node)
        {
            switch (node)
            {
                case SingleVariableDesignationSyntax s:
                    return Analyze(semanticModel, s.Identifier);

                case ParenthesizedVariableDesignationSyntax s:
                    return s.Variables.SelectMany(_ => Analyze(semanticModel, _));

                default:
                    return Enumerable.Empty<Diagnostic>();
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

        private IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, params SyntaxToken[] identifiers)
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