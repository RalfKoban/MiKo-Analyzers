using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2049_WillBePhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2049";

        internal static readonly IDictionary<string, string> PhrasesMap = new Dictionary<string, string>
                                                                              {
                                                                                  { "will be", "is" },
                                                                                  { "will also be", "is" },
                                                                                  { "will as well be", "is" },
                                                                                  { "will not be", "is not" },
                                                                                  { "will not", "does not" },
                                                                                  { "will never be", "is never" },
                                                                                  { "will never", "does never" },
                                                                                  { "will", "does" },
                                                                              };

        private static readonly string[] Phrases = GetWithDelimiters(PhrasesMap.Keys.ToArray());

        public MiKo_2049_WillBePhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, Phrases, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}