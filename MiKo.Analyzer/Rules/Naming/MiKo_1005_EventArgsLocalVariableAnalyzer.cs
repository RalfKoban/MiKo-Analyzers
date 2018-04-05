using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsEventArgs() == true;

        protected override IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> results = null;

            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;
                switch (name)
                {
                    case "e":
                    case "args":
                        break;

                    default:
                        var symbol = identifier.GetSymbol(semanticModel);

                        if (results == null) results = new List<Diagnostic>();
                        results.Add(ReportIssue(symbol, "e"));
                        break;
                }
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}