using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1015_InitMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1015";

        private const string Name = "Initialize";

        public MiKo_1015_InitMethodsAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IMethodSymbol method) => GetExpectedName(method.Name);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;
            if (methodName.StartsWith("Init", StringComparison.Ordinal) is false)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (methodName.StartsWith(Name, StringComparison.Ordinal))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var expectedName = FindBetterName(symbol);

            return new[] { Issue(symbol, expectedName) };
        }

        private static string GetExpectedName(string methodName)
        {
            var i = 1;
            for (; i < methodName.Length; i++)
            {
                var character = methodName[i];
                if (character.IsUpperCase())
                {
                    break;
                }

                if (character.IsNumber())
                {
                    break;
                }
            }

            return i >= methodName.Length ? Name : Name + methodName.Substring(i);
        }
    }
}