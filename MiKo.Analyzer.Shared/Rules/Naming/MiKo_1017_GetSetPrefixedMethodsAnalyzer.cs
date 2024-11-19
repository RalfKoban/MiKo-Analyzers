using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1017_GetSetPrefixedMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1017";

        private static readonly string[] StrangePrefixes =
                                                           {
                                                               "GetCan",
                                                               "GetHas",
                                                               "GetIs",
                                                               "GetExists",
                                                               "SetCan",
                                                               "SetHas",
                                                               "SetIs",
                                                               "SetExists",
                                                               "CanHas",
                                                               "CanIs",
                                                               "CanExists",
                                                               "HasCan",
                                                               "HasIs",
                                                               "HasExists",
                                                               "IsCan",
                                                               "IsHas",
                                                               "IsExists",
                                                           };

        public MiKo_1017_GetSetPrefixedMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (StrangePrefixes.Exists(_ => HasStrangePrefix(symbol, _)))
            {
                var betterName = FindBetterName(symbol.Name);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name)
        {
            var startIndex = 1;

            while (startIndex < name.Length)
            {
                if (name[startIndex].IsUpperCase())
                {
                    break;
                }

                startIndex++;
            }

            return name.Substring(startIndex);
        }

        private static bool HasStrangePrefix(IMethodSymbol method, string prefix)
        {
            var methodName = method.Name;

            if (methodName.Length == prefix.Length || (methodName.Length > prefix.Length && methodName[prefix.Length].IsUpperCase()))
            {
                if (methodName.StartsWith(prefix, StringComparison.Ordinal))
                {
                    if (method.HasDependencyObjectParameter() is false)
                    {
                        if (methodName.Contains("CanOpen") && method.ContainingNamespace.FullyQualifiedName().Contains("CanOpen"))
                        {
                            // it is a special CanOPEN protocol situation
                            return false;
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}