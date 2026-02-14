using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1040_ParameterCollectionSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1040";

        public MiKo_1040_ParameterCollectionSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol?.Parameters.Length > 0 && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol?.Type.IsCollection(symbol.Name) is true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var parameters = symbol.Parameters;

            foreach (var parameter in parameters)
            {
                if (parameter.Ordinal is 0 && symbol.IsExtensionMethod)
                {
                    // this is a 'this' parameter
                    continue;
                }

                if (ShallAnalyze(parameter))
                {
                    var issue = AnalyzeCollectionSuffix(parameter);

                    if (issue is null)
                    {
                        continue;
                    }

                    if (issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.BetterName, out var betterName) && parameters.Any(_ => _.Name == betterName))
                    {
                        // ignore duplicate names
                        continue;
                    }

                    yield return issue;
                }
            }
        }
    }
}