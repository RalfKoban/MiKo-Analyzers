using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1065_OperatorParameterNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1065";

        public MiKo_1065_OperatorParameterNameAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            switch (symbol.MethodKind)
            {
                case MethodKind.UserDefinedOperator:
                case MethodKind.Conversion: // that's an unary operator, such as an implicit conversion call
                    return true;

                default:
                    return false;
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeParameters(symbol.Parameters).Where(_ => _ != null);

        private IEnumerable<Diagnostic> AnalyzeParameters(ImmutableArray<IParameterSymbol> parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    yield return AnalyzeParameter(parameters[0], "value");

                    break;

                case 2:
                    yield return AnalyzeParameter(parameters[0], "left");
                    yield return AnalyzeParameter(parameters[1], "right");

                    break;
            }
        }

        private Diagnostic AnalyzeParameter(IParameterSymbol parameter, string name) => parameter.Name == name
                                                                                            ? null
                                                                                            : Issue(parameter, name);
    }
}