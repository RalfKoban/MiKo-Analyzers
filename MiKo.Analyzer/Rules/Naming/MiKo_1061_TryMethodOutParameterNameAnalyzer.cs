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

        internal static string FindBetterName(IMethodSymbol method) => GetPreferredParameterName(method.Name);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            return symbol.IsTestClass()
                       ? Enumerable.Empty<Diagnostic>() // ignore tests
                       : symbol.GetMembers().OfType<IMethodSymbol>().Select(AnalyzeTryMethod).Where(_ => _ != null);
        }

        private static string GetPreferredParameterName(string methodName)
        {
            if (methodName.StartsWith("TryGet", StringComparison.Ordinal))
            {
                var parameterName = methodName.Substring(6);
                if (parameterName.Length == 0)
                {
                    return "value";
                }

                var characters = parameterName.ToCharArray();
                characters[0] = char.ToLower(characters[0]);
                return string.Intern(new string(characters));
            }

            return "result";
        }

        private Diagnostic AnalyzeTryMethod(IMethodSymbol method) => method.Name.StartsWith("Try", StringComparison.Ordinal)
                                                                         ? AnalyzeOutParameter(method)
                                                                         : null;

        private Diagnostic AnalyzeOutParameter(IMethodSymbol method)
        {
            var parameterName = FindBetterName(method);

            var outParameter = method.Parameters.FirstOrDefault(_ => _.RefKind == RefKind.Out);
            if (outParameter != null && outParameter.Name != parameterName)
            {
                return Issue(outParameter, parameterName);
            }

            return null;
        }
    }
}