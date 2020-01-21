using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1027";

        public MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer() : base(Id, (SymbolKind)(-1), Constants.MaxNamingLengths.LocalVariablesInLoops)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(semanticModel, identifiers);

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, IEnumerable<SyntaxToken> identifiers)
        {
            List<Diagnostic> results = null;

            foreach (var identifier in identifiers)
            {
                var exceeding = GetExceedingCharacters(identifier.ValueText);
                if (exceeding <= 0)
                {
                    continue;
                }

                var symbol = identifier.GetSymbol(semanticModel);

                if (results is null)
                {
                    results = new List<Diagnostic>(1);
                }

                results.Add(Issue(symbol, exceeding));
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}