using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

        private static bool TagCommentHasIssue(string commentXml, string xmlTag) => GetCommentElements(commentXml, xmlTag).SelectMany(_ => _.Nodes()).Any(CommentHasIssue);

        private static bool CommentHasIssue(XNode node)
        {
            string comment;

            if (node is XElement e)
            {
                // skip <c> and <code>
                switch (e.Name.ToString().ToLower())
                {
                    case "c":
                    case "code":
                        return false;

                    default:
                    {
                        if (e.HasElements)
                            return e.Descendants().Any(CommentHasIssue);

                        // sentence starts lower case
                        comment = e.Value.TrimStart();

                        if (comment.Length > 0 && comment[0].IsLowerCaseLetter())
                            return true;

                        break;
                    }
                }
            }
            else
            {
                comment = node.ToString();
            }

            return CommentHasIssue(comment);
        }

        private static bool CommentHasIssue(string comment)
        {
            var commentLength = comment.Length;
            var last = commentLength - 1;

            for (var i = 0; i < commentLength; i++)
            {
                var c = comment[i];

                SkipWhiteSpaces(comment, last, ref c, ref i);

                if (c.IsSentenceEnding())
                {
                    // get next character after . ? or !
                    if (i != last)
                        c = comment[++i];

                    SkipWhiteSpaces(comment, last, ref c, ref i);
                    SkipAbbreviations(comment, last, ref c, ref i);

                    if (c.IsLowerCaseLetter())
                        return true;
                }
            }

            return false;
        }

        private static void SkipWhiteSpaces(string comment, int last, ref char c, ref int i)
        {
            while (c.IsWhiteSpace() && i < last)
            {
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