﻿using System;
using System.Collections.Generic;
using System.Text;

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
            var builder = name.AsCachedBuilder()
                                              .Insert(0, "original")
                                              .Without("toCopy")
                                              .Without("ToCopy");

            if (builder.Length > 8)
            {
                builder.ToUpperCaseAt(8);
            }

            return builder.ToStringAndRelease();
        }
    }
}