using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2201";

        private static readonly string[] XmlTags =
            {
                Constants.XmlTag.Example,
                Constants.XmlTag.Exception,
                Constants.XmlTag.Note,
                Constants.XmlTag.Overloads,
                Constants.XmlTag.Para,
                Constants.XmlTag.Param,
                Constants.XmlTag.Permission,
                Constants.XmlTag.Remarks,
                Constants.XmlTag.Returns,
                Constants.XmlTag.Summary,
                Constants.XmlTag.TypeParam,
                Constants.XmlTag.Value,
            };

        public MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            foreach (var xmlTag in XmlTags.Where(_ => TagCommentHasIssue(commentXml, _)))
            {
                yield return ReportIssue(symbol, xmlTag);
            }
        }

        private static bool TagCommentHasIssue(string commentXml, string xmlTag) => GetCommentElements(commentXml, xmlTag).Select(_ => _.Nodes().ConcatenatedWith()).Any(CommentHasIssue);

        private static bool CommentHasIssue(string comment)
        {
            var commentLength = comment.Length;
            var last = commentLength - 1;

            for (var i = 0; i < commentLength; i++)
            {
                var c = comment[i];

                SkipWhiteSpacesAndNestedXml(comment, last, ref c, ref i);

                if (c.IsSentenceEnding())
                {
                    // get next character after . ? or !
                    if (i != last)
                        c = comment[++i];

                    SkipWhiteSpacesAndNestedXml(comment, last, ref c, ref i);
                    SkipAbbreviations(comment, last, ref c, ref i);

                    if (c.IsLowerCaseLetter())
                        return true;
                }
            }

            return false;
        }

        private static void SkipWhiteSpacesAndNestedXml(string comment, int last, ref char c, ref int i)
        {
            SkipWhiteSpaces(comment, last, ref c, ref i);
            SkipNestedXml(comment, last, ref c, ref i);
        }

        private static void SkipWhiteSpaces(string comment, int last, ref char c, ref int i)
        {
            while (c.IsWhiteSpace() && i < last)
            {
                c = comment[++i];
            }
        }

        private static void SkipNestedXml(string comment, int last, ref char c, ref int i)
        {
            if (c == '<')
            {
                while (c != '>' && i < last)
                    c = comment[++i];
            }
        }

        private static void SkipAbbreviations(string comment, int last, ref char c, ref int i)
        {
            // for example in string "e.g." -> c is already 'g', as well as i
            const int Gap = 2;
            if (c.IsLowerCaseLetter() && i + Gap < last && comment[i + 1] == '.')
            {
                i += Gap;
                c = comment[i];
            }
        }
    }
}