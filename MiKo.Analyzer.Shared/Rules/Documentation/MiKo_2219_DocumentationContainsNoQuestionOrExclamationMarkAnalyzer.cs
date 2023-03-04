using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2219_DocumentationContainsNoQuestionOrExclamationMarkAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2219";

        internal static readonly string[] Terms = { "?", "!" };

        private static readonly HashSet<string> AllowedTags = new HashSet<string>
                                                                  {
                                                                      Constants.XmlTag.Code,
                                                                      Constants.XmlTag.Note,
                                                                  };

        public MiKo_2219_DocumentationContainsNoQuestionOrExclamationMarkAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.DescendantNodes<XmlTextSyntax>(_ => _.Ancestors<XmlElementSyntax>().None(__ => AllowedTags.Contains(__.GetName()))).SelectMany(_ => _.TextTokens))
            {
                foreach (var location in GetAllLocations(token, Terms))
                {
                    var word = location.GetSurroundingWord();

                    if (word.IsHyperlink())
                    {
                        continue;
                    }

                    yield return Issue(location);
                }
            }
        }
    }
}