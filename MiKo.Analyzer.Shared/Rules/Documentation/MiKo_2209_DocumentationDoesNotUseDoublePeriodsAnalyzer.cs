using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2209";

        private static readonly HashSet<char> AllowedChars = new HashSet<char>
                                                                 {
                                                                     '.',
                                                                     '/',
                                                                     '\\',
                                                                 };

        public MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                foreach (var location in GetAllLocations(token, "..", _ => AllowedChars.Contains(_) is false))  // we want to underline the first and last char
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}