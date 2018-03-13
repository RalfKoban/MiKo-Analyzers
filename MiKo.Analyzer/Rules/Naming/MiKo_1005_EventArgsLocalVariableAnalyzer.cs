using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1005_EventArgsLocalVariableAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1005";

        public MiKo_1005_EventArgsLocalVariableAnalyzer() : base(Id, (SymbolKind)(-1))
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
            var node = (VariableDeclarationSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            if (!ShallAnalyze(node, semanticModel)) return;

            var diagnostics = Analyze(node, semanticModel);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool ShallAnalyze(VariableDeclarationSyntax node, SemanticModel semanticModel) => semanticModel.GetTypeInfo(node.Type).Type?.IsEventArgs() == true;

        private IEnumerable<Diagnostic> Analyze(VariableDeclarationSyntax node, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var variable in node.Variables)
            {
                var name = variable.Identifier.ValueText;
                switch (name)
                {
                    case "e":
                        break;

                    default:
                        if (results == null) results = new List<Diagnostic>();
                        var symbol = semanticModel.LookupSymbols(variable.Identifier.GetLocation().SourceSpan.Start, name:name).First();
                        results.Add(ReportIssue(symbol, name, "e"));
                        break;
                }
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}