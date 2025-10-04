using System.Collections.Generic;

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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Length > 0 && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyze(IParameterSymbol symbol)
        {
            var parameterType = symbol.Type;

            if (parameterType.IsString())
            {
                return symbol.Name.EndsWithAny(Constants.Markers.Collections);
            }

            if (parameterType.IsXmlNode())
            {
                return false;
            }

            return parameterType.IsEnumerable();
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var parameter in symbol.Parameters)
            {
                if (parameter.Ordinal is 0 && symbol.IsExtensionMethod)
                {
                    // this is a 'this' parameter
                    continue;
                }

                if (ShallAnalyze(parameter))
                {
                    var issue = AnalyzeCollectionSuffix(parameter);

                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }
    }
}