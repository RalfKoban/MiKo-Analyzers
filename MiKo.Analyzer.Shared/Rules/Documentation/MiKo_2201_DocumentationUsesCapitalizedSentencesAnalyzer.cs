using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                ".config",
                ".cs",
                ".dll",
                ".eds",
                ".gif",
                ".htm",
                ".jpeg",
                ".jpg",
                ".png",
                ".resx",
                ".txt",
                ".xaml",
                ".xml",
            };

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                           {
                                                               Constants.XmlTag.Overloads,
                                                               Constants.XmlTag.Summary,
                                                               Constants.XmlTag.Remarks,
                                                               Constants.XmlTag.Param,
                                                               Constants.XmlTag.Returns,
                                                               Constants.XmlTag.Value,
                                                               Constants.XmlTag.Exception,
                                                               Constants.XmlTag.TypeParam,
                                                               Constants.XmlTag.Example,
                                                               Constants.XmlTag.Note,
                                                               Constants.XmlTag.Para,
                                                               Constants.XmlTag.Permission,
                                                           };

        public MiKo_2201_DocumentationUsesCapitalizedSentencesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            return AnalyzeComment(comment).Distinct(DiagnosticLocationEqualityComparer.Default);
        }

        private IEnumerable<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment)
        {
            foreach (var element in comment.DescendantNodes<XmlElementSyntax>())
            {
                var elementName = element.GetName();

                if (Tags.Contains(elementName) is false)
                {
                    continue;
                }

                // let's delve into all tokens that are part of a specific element (or any contained elements)
                var tokens = element.DescendantNodes(_ => _.IsCode() is false && _.IsC() is false, true)
                                    .OfType<XmlTextSyntax>()
                                    .SelectMany(_ => _.TextTokens)
                                    .Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken))
                                    .Where(_ =>
                                               {
                                                   var text = _.ValueText;

                                                   // skip empty texts here
                                                   return text.Length > 2 && text.IsNullOrWhiteSpace() is false;
                                               })
                                    .ToList();

                var previousText = string.Empty;
                foreach (var token in tokens)
                {
                    var text = token.ValueText;

                    if (previousText.TrimEnd().EndsWithAny(Constants.SemiSentenceMarkers))
                    {
                        // special case: see if previous token ended with a dot
                        const int Finding = 0;
                        var position = GetIssuePosition(text, Finding, token.SpanStart);
                        if (position != -1)
                        {
                            if (SkipFinding(text, Finding))
                            {
                                continue;
                            }

                            var location = CreateLocation(token, position, position + 1);

                            yield return Issue(location, elementName);
                        }
                    }

                    previousText = text;

                    var findings = text.AllIndexesOfAny(Constants.SemiSentenceMarkers);

                    foreach (var finding in findings)
                    {
                        var position = GetIssuePosition(text, finding, token.SpanStart);
                        if (position == -1)
                        {
                            // no issue
                            continue;
                        }

                        if (SkipFinding(text, finding))
                        {
                            continue;
                        }

                        var location = CreateLocation(token, position, position + 1);

                        yield return Issue(location, elementName);
                    }
                }
            }
        }

        private static bool SkipFinding(string text, int finding)
        {
            if (IsWellknownFileExtension(text, finding))
            {
                // seems we found a well-known file extension
                return true;
            }

            if (IsAbbreviation(text, finding))
            {
                // seems we found an well-known abbreviation
                return true;
            }

            return false;
        }

        private static bool IsWellknownFileExtension(string comment, int dotPosition) => comment.Substring(dotPosition).StartsWithAny(WellknownFileExtensions);

        private static bool IsAbbreviation(string comment, int dotPosition)
        {
            var nextDotPosition = dotPosition + 2;

            var abbreviation = "    ";

            // for example in string "e.g.": c is already 'g', as well as i
            if (comment.Length > nextDotPosition && dotPosition > 0)
            {
                if (comment[nextDotPosition] == '.')
                {
                    // we might be on the first dot
                    abbreviation = comment.Substring(dotPosition - 1, 4);
                }
                else
                {
                    // we might be on the second dot
                    abbreviation = comment.Substring(dotPosition - 3, 4);
                }
            }

            var c0 = abbreviation[0];
            var c1 = abbreviation[1];
            var c2 = abbreviation[2];
            var c3 = abbreviation[3];

            return c0.IsLowerCaseLetter() && c1 == '.' && c2.IsLowerCaseLetter() && c3 == '.';
        }

        private static int GetIssuePosition(string text, int finding, int spanStart)
        {
            var remainingText = text.Substring(finding);

            for (var index = 0; index < remainingText.Length; index++)
            {
                var c = remainingText[index];

                if (c.IsWhiteSpace())
                {
                    continue;
                }

                if (c.IsUpperCase())
                {
                    // its an upper case sentence, so no issue
                    return -1;
                }

                if (c.IsLowerCaseLetter())
                {
                    return spanStart + remainingText.Length + index;
                }
            }

            return -1;
        }
    }
}