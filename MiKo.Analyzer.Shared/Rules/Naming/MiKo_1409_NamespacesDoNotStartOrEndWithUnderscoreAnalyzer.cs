using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1409_NamespacesDoNotStartOrEndWithUnderscoreAnalyzer : NamespaceNamingAnalyzer
    {
        public const string Id = "MiKo_1409";

        private static readonly ConcurrentDictionary<string, string> BetterNames = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public MiKo_1409_NamespacesDoNotStartOrEndWithUnderscoreAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames)
        {
            List<Diagnostic> issues = null;

            foreach (var namePart in namespaceNames)
            {
                var name = namePart.ValueText;

                if (HasIssue(name.AsSpan(), Constants.Underscore))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(name, namePart, FindBetterName(name)));
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        private static bool HasIssue(in ReadOnlySpan<char> name, in char character)
        {
            if (name.Length is 0)
            {
                // code seems to be obfuscated or contains no valid characters, so ignore it silently
                return false;
            }

            if (name[0] == character)
            {
                var trimmed = name.TrimStart(character);

                // namespaces starting with numbers need to be "escaped", so this situation is no issue
                return trimmed.Length <= 0 || trimmed[0].IsNumber() is false;
            }

            return name[name.Length - 1] == character;
        }

        private static string FindBetterName(string name) => BetterNames.GetOrAdd(name, _ => _.Trim(Constants.Underscores));
    }
}