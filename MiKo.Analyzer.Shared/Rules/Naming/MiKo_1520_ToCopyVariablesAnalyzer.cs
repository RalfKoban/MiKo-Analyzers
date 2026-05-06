using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1520_ToCopyVariablesAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1520";

        public MiKo_1520_ToCopyVariablesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, length = identifiers.Length; index < length; index++)
            {
                var identifier = identifiers[index];
                var name = identifier.ValueText;
                var nameSpan = name.AsSpan();

                if (nameSpan.StartsWith("toCopy") || nameSpan.EndsWith("ToCopy"))
                {
                    var betterName = FindBetterName(name);

                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(name, identifier, betterName, CreateBetterNameProposal(betterName)));
                }
            }

            return issues ?? Enumerable.Empty<Diagnostic>();
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