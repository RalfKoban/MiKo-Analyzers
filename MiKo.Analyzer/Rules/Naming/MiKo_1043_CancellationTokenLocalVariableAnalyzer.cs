using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1043_CancellationTokenLocalVariableAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1043";

        private const string Name = "token";

        public MiKo_1043_CancellationTokenLocalVariableAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDeclarationPattern, SyntaxKind.DeclarationPattern);
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsCancellationToken() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> results = null;

            foreach (var identifier in identifiers)
            {
                if (identifier.ValueText == Name) continue;

                var symbol = identifier.GetSymbol(semanticModel);

                if (results == null) results = new List<Diagnostic>();
                results.Add(Issue(symbol, Name));
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}