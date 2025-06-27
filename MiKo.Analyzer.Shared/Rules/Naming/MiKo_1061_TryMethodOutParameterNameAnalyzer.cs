using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1061_TryMethodOutParameterNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1061";

        public MiKo_1061_TryMethodOutParameterNameAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => symbol.IsTestClass()
                                                                                                                    ? Array.Empty<Diagnostic>() // ignore tests
                                                                                                                    : symbol.GetNamedMethods().Select(AnalyzeTryMethod).WhereNotNull();

        private static string GetPreferredParameterName(string methodName)
        {
            const string TryGet = "TryGet";

            if (methodName.StartsWith(TryGet, StringComparison.Ordinal))
            {
                var parameterName = methodName.AsSpan(TryGet.Length);

                if (parameterName.Length is 0)
                {
                    return "value";
                }

                var name = parameterName.ToLowerCaseAt(0);

                if (CodeDetector.IsCSharpKeyword(name) is false)
                {
                    return name;
                }
            }

            return "result";
        }

        private Diagnostic AnalyzeTryMethod(IMethodSymbol method) => method.Name.StartsWith("Try", StringComparison.Ordinal)
                                                                     ? AnalyzeOutParameter(method)
                                                                     : null;

        private Diagnostic AnalyzeOutParameter(IMethodSymbol method)
        {
            var parameters = method.Parameters;
            var parametersLength = parameters.Length;

            if (parametersLength is 0)
            {
                return null;
            }

            var outParameterIndex = -1;

            for (var index = 0; index < parametersLength; index++)
            {
                if (parameters[index].RefKind is RefKind.Out)
                {
                    if (outParameterIndex is -1)
                    {
                        outParameterIndex = index;
                    }
                    else
                    {
                        // we do not support multiple out parameters
                        return null;
                    }
                }
            }

            if (outParameterIndex is -1)
            {
                return null;
            }

            var outParameter = parameters[outParameterIndex];
            var parameterName = GetPreferredParameterName(method.Name);

            if (outParameter.Name == parameterName)
            {
                return null;
            }

            return Issue(outParameter, parameterName, CreateBetterNameProposal(parameterName));
        }
    }
}