using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2223";

        private const string TrimChars = ".?!;:,'\"()[]{}%";

        private static readonly char[] SpecialIndicators =
                                                           {
                                                               '*',  // seems to be a file extension
                                                               '+',  // seems to be a shortcut
                                                               '-',  // seems to be an abbreviation
                                                               '/',  // seems to be a combined word
                                                               '\\', // seems to be a path word
                                                               ':', // seems to be a 'suppress message' text
                                                               '=', // seems to be something like 'PublicKeyToken=1234'
                                                               '#',
                                                           };

        private static readonly string[] HyperlinkIndicators = { "http:", "https:", "ftp:", "ftps:" };

        private static readonly string[] CompilerWarningIndicators = { "CS", "CA", "SA" };

        private static readonly HashSet<string> IgnoreTags = new HashSet<string>
                                                                 {
                                                                     Constants.XmlTag.Code,
                                                                     Constants.XmlTag.C,
                                                                     Constants.XmlTag.See,
                                                                     Constants.XmlTag.SeeAlso,
                                                                     "a",
                                                                 };

        private static readonly HashSet<string> WellKnownWords = new HashSet<string>
                                                                     {
                                                                         "CSharp",
                                                                         "FxCop",
                                                                         "IntelliSense",
                                                                         "NCover",
                                                                         "NCrunch",
                                                                         "PostSharp",
                                                                         "ReSharper",
                                                                         "SonarCube",
                                                                         "SonarLint",
                                                                         "SonarQube",
                                                                         "StyleCop",
                                                                         "VisualBasic",
                                                                         "ASP.NET",
                                                                     };

        public MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(comment);

        private static bool TryFindCompoundWord(string text, ref int startIndex, ref int endIndex)
        {
            // jump over white-spaces at the beginning
            for (; startIndex < text.Length; startIndex++)
            {
                var c = text[startIndex];

                if (c.IsWhiteSpace() is false)
                {
                    // no white-space, so break
                    break;
                }
            }

            endIndex = startIndex;

            var foundUpperCaseLetters = 0;

            // now find next white-space and count upper cases in between
            for (; endIndex < text.Length; endIndex++)
            {
                var c = text[endIndex];

                if (c.IsUpperCaseLetter())
                {
                    // we found an upper case
                    foundUpperCaseLetters++;

                    continue;
                }

                if (c.IsWhiteSpace())
                {
                    // we found another white-space, so we are finished with the current word, so break
                    break;
                }
            }

            // we found an compound word
            return foundUpperCaseLetters > 1;
        }

        private IEnumerable<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens(_ => IgnoreTags.Contains(_.GetName()) is false))
            {
                var text = token.ValueText;

                if (text.Length < 3)
                {
                    // ignore small texts as they do not contain compound words
                    continue;
                }

                foreach (var diagnostic in AnalyzeComment(token, text))
                {
                    yield return diagnostic;
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeComment(SyntaxToken token, string text)
        {
            var textLength = text.Length - 1;

            var start = 0;
            var end = -1;

            // loop over complete text
            while (end < textLength)
            {
                if (TryFindCompoundWord(text, ref start, ref end))
                {
                    // get rid of leading or trailing additional characters such as braces or sentence markers
                    var span = text.AsSpan(start, end - start);
                    var trimmedStart = span.TrimStart(TrimChars.AsSpan());
                    var trimmedEnd = trimmedStart.Contains('(') && span.EndsWith(')')
                                         ? span
                                         : span.TrimEnd(TrimChars.AsSpan());

                    start += span.Length - trimmedStart.Length;
                    end -= span.Length - trimmedEnd.Length;

                    var trimmed = text.AsSpan(start, end - start);

                    var compoundWord = true;

                    if (trimmed.Length == 0)
                    {
                        compoundWord = false;
                    }
                    else
                    {
                        if (trimmed[0].IsNumber())
                        {
                            compoundWord = false;
                        }
                        else if (trimmed.Length > 3 && trimmed[2].IsNumber() && trimmed.StartsWithAny(CompilerWarningIndicators, StringComparison.Ordinal))
                        {
                            compoundWord = false;
                        }
                        else if (trimmed.StartsWithAny(HyperlinkIndicators, StringComparison.OrdinalIgnoreCase))
                        {
                            compoundWord = false;
                        }
                        else if (trimmed.ContainsAny(SpecialIndicators))
                        {
                            compoundWord = false;
                        }
                        else if (trimmed.Length > 31 && Guid.TryParse(trimmed.ToString(), out _))
                        {
                            compoundWord = false;
                        }
                        else if (trimmed.All(char.IsUpper))
                        {
                            // seems like an abbreviation such as UML, so do not report
                            compoundWord = false;
                        }
                        else if (trimmed.EndsWith('s'))
                        {
                            var characters = trimmed.EndsWith("'s", StringComparison.Ordinal) ? 2 : 1;

                            var part = trimmed.Slice(0, trimmed.Length - characters).ToString();

                            if (part.All(char.IsUpper) || WellKnownWords.Contains(part))
                            {
                                // seems like an abbreviation (such as UIs) or a genitive tense of an abbreviation (such as UI's), so do not report
                                compoundWord = false;
                            }
                        }
                        else if (WellKnownWords.Contains(trimmed.ToString()))
                        {
                            compoundWord = false;
                        }
                    }

                    if (compoundWord && trimmed.StartsWith("default(", StringComparison.Ordinal) && trimmed.EndsWith(')') is false)
                    {
                        // adjust the default to include the brace as it had been trimmed above
                        var i = text.IndexOf(')');
                        if (i != -1)
                        {
                            end = i + 1;
                        }
                    }

                    if (compoundWord)
                    {
                        // we found a compound word, so report that
                        var location = CreateLocation(token, token.SpanStart + start, token.SpanStart + end);

                        yield return Issue(location);
                    }
                }

                // jump over word
                start = end;
            }
        }
    }
}