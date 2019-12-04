using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1043_CancellationTokenLocalVariableAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1043";

        private const string Name = "token";

        public MiKo_1043_CancellationTokenLocalVariableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsCancellationToken() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> results = null;

            foreach (var identifier in identifiers)
            {
                if (identifier.ValueText == Name)
                {
                    continue;
                }

                var symbol = identifier.GetSymbol(semanticModel);

                if (results is null)
                {
                    results = new List<Diagnostic>(1);
                }

                results.Add(Issue(symbol, Name));
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}