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
                Constants.XmlTag.Summary,
                Constants.XmlTag.Remarks,
                Constants.XmlTag.Returns,
                Constants.XmlTag.Value,
                Constants.XmlTag.Param,
                Constants.XmlTag.Exception,
                Constants.XmlTag.TypeParam,
                Constants.XmlTag.Example,
                Constants.XmlTag.Note,
                Constants.XmlTag.Overloads,
                Constants.XmlTag.Para,
                Constants.XmlTag.Permission,
            };

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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            var element = commentXml.GetCommentElement();

            return XmlTags.Where(_ => element.GetCommentElements(_).SelectMany(__ => __.Nodes()).Any(CommentHasIssue)).Select(_ => Issue(symbol, _));
        }

        private static bool CommentHasIssue(XNode node)
        {
            string comment;

            if (node is XElement e)
            {
                // skip <c> and <code>
                var name = e.Name.ToString().ToLowerCase();
                switch (name)
                {
                    case Constants.XmlTag.C:
                    case Constants.XmlTag.Code:
                        return false;

                    default:
                    {
                        if (e.HasElements)
                        {
                            return e.Descendants().Any(CommentHasIssue);
                        }

                        comment = e.Value.TrimStart();

                        // sentence starts lower case
                        if (name == Constants.XmlTag.Para && comment.Length > 0 && comment[0].IsLowerCaseLetter())
                        {
                            return true;
                        }

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
                    // investigate next character after . ? or !
                    if (i != last)
                    {
                        c = comment[++i];
                    }

                    SkipWhiteSpaces(comment, last, ref c, ref i);
                    SkipAbbreviations(comment, last, ref c, ref i);

                    if (c.IsLowerCaseLetter() && IsWellknownFileExtension(comment, i - 1) is false)
                    {
                        return true;
                    }
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