using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1521_ToCopyParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1521";

        public MiKo_1521_ToCopyParametersAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var name = symbol.Name;
            var nameSpan = name.AsSpan();

            if (nameSpan.StartsWith("toCopy") || nameSpan.EndsWith("ToCopy"))
            {
                var betterName = FindBetterName(name);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name)
        {
            const string Original = "original";

            var builder = name.AsCachedBuilder()
                              .Insert(0, Original)
                              .Without("toCopy")
                              .Without("ToCopy");

            if (builder.Length > Original.Length)
            {
                builder.ToUpperCaseAt(Original.Length);
            }

            return builder.ToStringAndRelease();
        }
    }
}