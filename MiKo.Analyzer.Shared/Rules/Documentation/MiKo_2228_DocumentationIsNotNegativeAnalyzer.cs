using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2228_DocumentationIsNotNegativeAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2228";

        private static readonly string[] ProblematicWords = { "n't", " not ", "cannot", "Cannot", " cant ", " wont ", "ouldnt" };

        public MiKo_2228_DocumentationIsNotNegativeAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            return Enumerable.Empty<Diagnostic>()
                             .Concat(AnalyzeXml(comment.GetSummaryXmls()))
                             .Concat(AnalyzeXml(comment.GetRemarksXmls()));
        }

        private static bool SentenceHasIssue(XmlElementSyntax xml)
        {
            var text = xml.GetTextTrimmed();

            foreach (var sentence in text.AsSpan().SplitBy(Constants.SentenceMarkers))
            {
                var indices = new HashSet<int>();

                foreach (var problematicWord in ProblematicWords)
                {
                    indices.AddRange(sentence.Text.AllIndicesOf(problematicWord, StringComparison.Ordinal));

                    if (indices.Count > 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private IEnumerable<Diagnostic> AnalyzeXml(IEnumerable<XmlElementSyntax> xmls)
        {
            foreach (var xml in xmls)
            {
                if (SentenceHasIssue(xml))
                {
                    yield return Issue(xml.StartTag);
                }
            }
        }
    }
}