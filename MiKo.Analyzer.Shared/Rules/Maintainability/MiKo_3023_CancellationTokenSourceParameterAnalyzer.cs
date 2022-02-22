using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3023_CancellationTokenSourceParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3023";

        public MiKo_3023_CancellationTokenSourceParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => from parameter in symbol.Parameters
                                                                                                             let parameterType = parameter.Type
                                                                                                             where parameterType.TypeKind == TypeKind.Class && parameterType.ToString() == TypeNames.CancellationTokenSource
                                                                                                             select Issue(parameter, nameof(CancellationToken));
    }
}