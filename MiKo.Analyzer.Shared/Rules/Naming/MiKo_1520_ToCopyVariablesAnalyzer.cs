using System;
using System.Collections.Generic;

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
            var length = identifiers.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var identifier = identifiers[index];
                    var name = identifier.ValueText;
                    var nameSpan = name.AsSpan();

                    if (nameSpan.StartsWith("toCopy") || nameSpan.EndsWith("ToCopy"))
                    {
                        var betterName = FindBetterName(name);

                        yield return Issue(name, identifier, betterName, CreateBetterNameProposal(betterName));
                    }
                }
            }
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