using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1042_CancellationTokenParameterNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1042";

        private const string ExpectedName = "cancellationToken";

        public MiKo_1042_CancellationTokenParameterNameAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Length > 0 && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var parameters = symbol.Parameters;
            var length = parameters.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var parameter = parameters[index];

                    if (parameter.Type.IsCancellationToken() && parameter.Name != ExpectedName)
                    {
                        yield return Issue(parameter, ExpectedName, CreateBetterNameProposal(ExpectedName));
                    }
                }
            }
        }
    }
}