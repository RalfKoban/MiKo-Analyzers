using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1016_GetSetPrefixedMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1016";

        private static readonly string[] Prefixes = { "GetCan", "GetHas", "GetIs", "SetCan", "SetHas", "SetIs", "CanHas", "CanIs", "HasCan", "HasIs", "IsCan", "IsHas" };

        public MiKo_1016_GetSetPrefixedMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeOrdinaryMethod(IMethodSymbol method) => Prefixes.Any(_ => HasStrangePrefix(method, _))
                                                                                                  ? new[] { ReportIssue(method, FindBetterName(method.Name)) }
                                                                                                  : Enumerable.Empty<Diagnostic>();

        private static string FindBetterName(string name)
        {
            var startIndex = 1;
            while (startIndex < name.Length)
            {
                if (name[startIndex].IsUpperCase())
                    break;

                startIndex++;
            }

            return name.Substring(startIndex);
        }

        private static bool HasStrangePrefix(IMethodSymbol method, string prefix)
        {
            var methodName = method.Name;

            return methodName.StartsWith(prefix, StringComparison.Ordinal)
                && methodName.Length > prefix.Length
                && methodName[prefix.Length].IsUpperCase()
                && !method.HasDependencyObjectParameter();
        }
    }
}