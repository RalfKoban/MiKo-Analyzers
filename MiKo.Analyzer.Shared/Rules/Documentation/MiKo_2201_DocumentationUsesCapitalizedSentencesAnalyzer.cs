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

        public MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var commentXml = symbol.GetDocumentationCommentXml();
            var element = commentXml.GetCommentElement();

            List<Diagnostic> results = null;

            foreach (var xmlTag in XmlTags)
            {
                if (element.GetCommentElements(xmlTag).SelectMany(__ => __.Nodes()).Any(CommentHasIssue))
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    results.Add(Issue(symbol, xmlTag));
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }

        private static bool CommentHasIssue(XNode node)
        {
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

                        var comment = e.Value.AsSpan().TrimStart();

                        // sentence starts lower case
                        if (name is Constants.XmlTag.Para && comment.Length > 0 && comment[0].IsLowerCaseLetter())
                        {
                            return true;
                        }

                        return CommentHasIssue(comment);
                    }
                }
            }

            return CommentHasIssue(node.ToString().AsSpan());
        }

        private static bool CommentHasIssue(in ReadOnlySpan<char> comment)
        {
            for (int i = 0, last = comment.Length - 1; i <= last; i++)
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

        private static void SkipWhiteSpaces(in ReadOnlySpan<char> comment, in int last, ref char c, ref int i)
        {
            while (c.IsWhiteSpace() && i < last)
            {
                c = comment[++i];
            }
        }

        private static void SkipAbbreviations(in ReadOnlySpan<char> comment, in int last, ref char c, ref int i)
        {
            // for example in string "e.g.": c is already 'g', as well as i
            const int Gap = 2;

            while (c.IsLowerCaseLetter())
            {
                var next = i + Gap;

                if (next < last && comment[i + 1] is '.')
                {
                    i = next;
                    c = comment[i];
                }
                else
                {
                    return;
                }
            }
        }

        private static bool IsWellknownFileExtension(in ReadOnlySpan<char> comment, in int startIndex) => comment.Slice(startIndex).StartsWithAny(Constants.WellknownFileExtensions, StringComparison.OrdinalIgnoreCase);
    }
}