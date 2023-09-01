using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1409_NamespacesDoNotStartOrEndWithUnderscoreAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1409";

        private const char Underscore = '_';

        private static readonly char[] Underscores = { Underscore };

        public MiKo_1409_NamespacesDoNotStartOrEndWithUnderscoreAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(string name) => name.Trim(Underscores);

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            foreach (var namePart in names)
            {
                var name = namePart.ValueText;

                if (HasIssue(name, Underscore))
                {
                    yield return Issue(name, namePart, FindBetterName(name));
                }
            }
        }

        private static bool HasIssue(string name, char character)
        {
            if (name.First() == character)
            {
                var trimmed = name.AsSpan().TrimStart(character);

                if (trimmed.Length > 0)
                {
                    if (trimmed[0].IsNumber())
                    {
                        // namespaces starting with numbers need to be "escaped", so this situation is no issue
                        return false;
                    }
                }

                return true;
            }

            return name.Last() == character;
        }
    }
}