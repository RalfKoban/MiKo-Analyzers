using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1530";

        private const string Prefix = "On";
        private const string Suffix = "Changed";

        private static readonly string[] ReplaceCandidates = { "Property", "Change" + Suffix, Suffix + Suffix };

        public MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsDependencyPropertyChangedCallback();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            var correctPrefix = methodName.StartsWith(Prefix, StringComparison.Ordinal);
            var correctSuffix = methodName.EndsWith(Suffix, StringComparison.Ordinal);

            if (correctPrefix && correctSuffix)
            {
                return Array.Empty<Diagnostic>();
            }

            var builder = methodName.AsCachedBuilder();

            if (correctPrefix is false)
            {
                builder = builder.Insert(0, Prefix); // prefix does not match
            }

            if (correctSuffix is false)
            {
                builder = builder.Append(Suffix); // suffix does not match
            }

            builder = builder.ReplaceAllWithProbe(ReplaceCandidates, Suffix);

            string betterName = builder.ToStringAndRelease();

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }
    }
}