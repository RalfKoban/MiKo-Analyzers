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

        private static readonly string[] Prefixes = { "GetCan", "GetHas", "GetIs", "SetCan", "SetHas", "SetIs" };

        public MiKo_1016_GetSetPrefixedMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeOrdinaryMethod(IMethodSymbol method) => Prefixes.Any(_ => HasStrangePrefix(method.Name, _))
                                                                                                  ? new []{ ReportIssue(method, method.Name.Substring(3)) }
                                                                                                  : Enumerable.Empty<Diagnostic>();

        private static bool HasStrangePrefix(string methodName, string prefix) => methodName.StartsWith(prefix, StringComparison.Ordinal)
                                                                               && methodName.Length > prefix.Length
                                                                               && methodName[prefix.Length].IsUpperCase();
    }
}