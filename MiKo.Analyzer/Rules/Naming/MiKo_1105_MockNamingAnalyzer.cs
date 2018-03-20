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

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeVariableDeclaration, SyntaxKind.VariableDeclaration);
        }

        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            var type = context.FindContainingType();
            if (type != null && type.IsTestClass())
            {
                var node = (VariableDeclarationSyntax)context.Node;

                AnalyzeVariableDeclaration(context, node);
            }
        }


        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context, VariableDeclarationSyntax node)
        {
            var semanticModel = context.SemanticModel;

            var diagnostics = Analyze(node, semanticModel);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> Analyze(VariableDeclarationSyntax node, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var identifier in node.Variables.Select(_ => _.Identifier))
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