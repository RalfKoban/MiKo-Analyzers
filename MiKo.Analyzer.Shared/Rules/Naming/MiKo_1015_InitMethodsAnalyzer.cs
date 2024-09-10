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

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.StartsWith("Init", StringComparison.Ordinal) && methodName.StartsWith(Name, StringComparison.Ordinal) is false)
            {
                var betterName = GetExpectedName(symbol.Name);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Enumerable.Empty<Diagnostic>();
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

            return i >= methodName.Length ? Name : Name.ConcatenatedWith(methodName.AsSpan(i));
        }
    }
}