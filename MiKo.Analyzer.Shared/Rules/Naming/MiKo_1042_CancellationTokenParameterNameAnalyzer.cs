using System.Collections.Generic;
using System.Linq;

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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Length > 0;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => from parameter in symbol.Parameters
                                                                                                                 where parameter.Type.IsCancellationToken() && parameter.Name != ExpectedName
                                                                                                                 select Issue(parameter, ExpectedName, CreateBetterNameProposal(ExpectedName));
    }
}