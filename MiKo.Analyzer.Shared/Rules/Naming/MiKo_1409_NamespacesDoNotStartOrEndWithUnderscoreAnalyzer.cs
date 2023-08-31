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

                if (name.First() == Underscore || name.Last() == Underscore)
                {
                    yield return Issue(name, namePart, FindBetterName(name));
                }
            }
        }
    }
}