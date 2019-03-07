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

                if (!c.IsSentenceEnding())
                    continue;

                // get next
                if (i != last)
                    c = comment[++i];

                // now check
                while (c.IsWhiteSpace() && i < last)
                    c = comment[++i];

                if (c.IsLetter() && c.IsLowerCase())
                    return true;
            }

            return false;
        }
    }
}