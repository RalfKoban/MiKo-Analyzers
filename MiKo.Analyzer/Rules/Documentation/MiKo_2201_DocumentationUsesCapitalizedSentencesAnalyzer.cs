using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2201";

        private static readonly string[] WellknownFileExtensions =
            {
                ".bmp",
                ".cs",
                ".dll",
                ".eds",
                ".gif",
                ".htm",
                ".jpeg",
                ".jpg",
                ".png",
                ".resx",
                ".xaml",
                ".xml",
            };

        public MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            if (HasIssue(comment))
            {
                yield return Issue(symbol);
            }
        }

        private static bool HasIssue(DocumentationCommentTriviaSyntax comment)
        {
            if (comment is null)
            {
                // it might be that there is no documentation comment available
                return false;
            }

            var elements = comment.DescendantNodes<XmlElementSyntax>().ToList();

            var hasIssue = Analyze(elements, Constants.XmlTag.Overloads)
                        || Analyze(elements, Constants.XmlTag.Summary)
                        || Analyze(elements, Constants.XmlTag.Remarks)
                        || Analyze(elements, Constants.XmlTag.Param)
                        || Analyze(elements, Constants.XmlTag.Returns)
                        || Analyze(elements, Constants.XmlTag.Value)
                        || Analyze(elements, Constants.XmlTag.Exception)
                        || Analyze(elements, Constants.XmlTag.TypeParam)
                        || Analyze(elements, Constants.XmlTag.Example)
                        || Analyze(elements, Constants.XmlTag.Note)
                        || Analyze(elements, Constants.XmlTag.Para)
                        || Analyze(elements, Constants.XmlTag.Permission);

            return hasIssue;
        }

        private static bool HasIssue(string text)
        {
            var sentences = text.Split(Constants.SentenceMarkers, StringSplitOptions.RemoveEmptyEntries);

            return sentences.Any(SentenceHasIssue);
        }

        private static bool Analyze(IEnumerable<XmlElementSyntax> nodes, string tagName) => nodes.Where(_ => _.GetName() == tagName).Select(ConstructComment).Any(HasIssue);

        private static bool SentenceHasIssue(string sentence)
        {
            var text = sentence.TrimStart();

            const int MinimumLength = 2; // we do not want to report abbreviations
            return text.Length >= MinimumLength && text[0].IsLowerCaseLetter();
        }

        private static void SkipAbbreviations(string comment, int last, ref char c, ref int i)
        {
            // for example in string "e.g.": c is already 'g', as well as i
            const int Gap = 2;

            if (c.IsLowerCaseLetter())
            {
                var next = i + Gap;
                if (next < last && comment[i + 1] == '.')
                {
                    i = next;
                    c = comment[i];
                }
            }
        }

        private static bool IsWellknownFileExtension(string comment, int startIndex) => comment.Substring(startIndex).StartsWithAny(WellknownFileExtensions);
    }
}