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

        private static readonly string[] LangwordCandidates = { "true", "false", "null" };

        private static readonly string[] ExampleCandidates = { "e.g", "i.e", "p.ex", "e.g.", "i.e.", "p.ex." };

        private static readonly string[] HyperlinkIndicators = { "http:", "https:", "ftp:", "ftps:" };

        private static readonly string[] CompilerWarningIndicators = { "CS", "CA", "SA" };

        private static readonly string[] StringEmptyTexts =
                                                            {
                                                                "sring.empty",
                                                                "sring.Empty",
                                                                "Sring.empty",
                                                                "Sring.Empty",
                                                                "sring.empy",
                                                                "sring.Empy",
                                                                "Sring.empy",
                                                                "Sring.Empy",
                                                                "sring.emtpy",
                                                                "sring.Emtpy",
                                                                "Sring.emtpy",
                                                                "Sring.Emtpy",
                                                                "sting.empty",
                                                                "sting.Empty",
                                                                "Sting.empty",
                                                                "Sting.Empty",
                                                                "sting.empy",
                                                                "sting.Empy",
                                                                "Sting.empy",
                                                                "Sting.Empy",
                                                                "sting.emtpy",
                                                                "sting.Emtpy",
                                                                "Sting.emtpy",
                                                                "Sting.Emtpy",
                                                                "string.empty",
                                                                "string.Empty",
                                                                "String.empty",
                                                                "String.Empty",
                                                                "string.empy",
                                                                "string.Empy",
                                                                "String.empy",
                                                                "String.Empy",
                                                                "string.emtpy",
                                                                "string.Emtpy",
                                                                "String.emtpy",
                                                                "String.Emtpy",
                                                            };

        private static readonly HashSet<string> IgnoreTags = new HashSet<string>
                                                                 {
                                                                     Constants.XmlTag.Code,
                                                                     Constants.XmlTag.C,
                                                                     Constants.XmlTag.See,
                                                                     Constants.XmlTag.SeeAlso,
                                                                     "a",
                                                                 };

        private static readonly string[] WellKnownWords =
                                                          {
                                                              "ASP.NET",
                                                              "CSharp",
                                                              "FxCop",
                                                              "IntelliSense",
                                                              "Microsoft",
                                                              "MSTest",
                                                              "NCover",
                                                              "NUnit",
                                                              "NCrunch",
                                                              "Outlook",
                                                              "PostSharp",
                                                              "ReSharper",
                                                              "SonarCube",
                                                              "SonarLint",
                                                              "SonarQube",
                                                              "StyleCop",
                                                              "VisualBasic",
                                                              "etc",
                                                              "xUnit",
                                                          };

        private static readonly string[] SingleWords =
                                                       {
                                                           "bool",
                                                           "byte",
                                                           "char",
                                                           "float",
                                                           "int",
                                                           "string",
                                                           "uint",
                                                           "ushort",
                                                           "ulong",
                                                           nameof(String),
                                                           nameof(Int16),
                                                           nameof(Int32),
                                                           nameof(Int64),
                                                           nameof(UInt16),
                                                           nameof(UInt32),
                                                           nameof(UInt64),
                                                           nameof(Single),
                                                           nameof(Double),
                                                           nameof(Boolean),
                                                           nameof(Byte),
                                                           nameof(Char),
                                                           nameof(Type),
                                                       };

        public MiKo_2223_DocumentationDoesNotUsePlainTextReferencesAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Location> alreadyReportedLocations = null;
            List<Diagnostic> results = null;

            var textTokens = comment.GetXmlTextTokens(_ => IgnoreTags.Contains(_.GetName()) is false);
            var textTokensCount = textTokens.Count;

            if (textTokensCount > 0)
            {
                for (var i = 0; i < textTokensCount; i++)
                {
                    var token = textTokens[i];
                    var text = token.ValueText;

                    if (text.Length < Constants.MinimumCharactersThreshold)
                    {
                        // ignore small texts as they do not contain compound words
                        continue;
                    }

                    // ReSharper disable once LoopCanBePartlyConvertedToQuery
                    foreach (var issue in AnalyzeComment(token, text))
                    {
                        if (issue is null)
                        {
                            continue;
                        }

                        if (alreadyReportedLocations is null)
                        {
                            alreadyReportedLocations = new List<Location>(1);
                        }
                        else
                        {
                            if (alreadyReportedLocations.Exists(_ => issue.Location.IntersectsWith(_)))
                            {
                                // already reported, so ignore it
                                continue;
                            }
                        }

                        alreadyReportedLocations.Add(issue.Location);

                        if (results is null)
                        {
                            results = new List<Diagnostic>(1);
                        }

                        results.Add(issue);
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }

        private static bool TryFindCompoundWord(in ReadOnlySpan<char> text, ref int startIndex, ref int endIndex)
        {
            var textLength = text.Length;

            // jump over white-spaces at the beginning
            for (; startIndex < textLength; startIndex++)
            {
                var c = text[startIndex];

                if (c.IsWhiteSpace() is false)
                {
                    // no white-space, so break
                    break;
                }
            }

            endIndex = startIndex;

            var findings = 0;

            // now find next white-space and count upper cases in between
            for (; endIndex < textLength; endIndex++)
            {
                var c = text[endIndex];

                if (c.IsUpperCaseLetter())
                {
                    // we found an upper case
                    findings++;

                    continue;
                }

                if (c is '.')
                {
                    // we found a dot which symbols a method or property call
                    findings++;

                    continue;
                }

                if (c.IsWhiteSpace())
                {
                    // we found another white-space, so we are finished with the current word, so break
                    break;
                }
            }

            // we found a compound word
            return findings > 1;
        }

        private static bool IsCompoundWord(in ReadOnlySpan<char> trimmed)
        {
            if (trimmed.Length is 0)
            {
                return false;
            }

            if (trimmed[0].IsNumber())
            {
                return false; // numbers as 1st character are no valid type names
            }

            if (trimmed.EqualsAny(ExampleCandidates, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (trimmed.Length > 3 && trimmed[2].IsNumber() && trimmed.StartsWithAny(CompilerWarningIndicators))
            {
                return false;
            }

            if (trimmed.EqualsAny(LangwordCandidates, StringComparison.OrdinalIgnoreCase))
            {
                // do not report stuff like 'true' as that is a langword which gets reported by MiKo_2040
                return false;
            }

            if (trimmed.StartsWithAny(HyperlinkIndicators, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (trimmed.ContainsAny(SpecialIndicators))
            {
                return false;
            }

            if (trimmed.EndsWithAny(Constants.WellknownFileExtensions, StringComparison.OrdinalIgnoreCase))
            {
                // do not report files
                return false;
            }

            if (trimmed.Length > 31 && Guid.TryParse(trimmed.ToString(), out _))
            {
                return false;
            }

            if (trimmed.AllUpper())
            {
                // seems like an abbreviation such as UML, so do not report
                return false;
            }

            if (trimmed.Any('!'))
            {
                return false;
            }

            if (trimmed.EndsWith('s'))
            {
                var characters = trimmed.EndsWith("'s") ? 2 : 1;

                var part = trimmed.Slice(0, trimmed.Length - characters);

                if (part.AllUpper() || IsWellKnownWord(part))
                {
                    // seems like an abbreviation (such as UIs) or a genitive tense of an abbreviation (such as UI's), so do not report
                    return false;
                }
            }
            else if (trimmed.EndsWith('d'))
            {
                var characters = trimmed.EndsWith("ed") ? 2 : 1;

                var part = trimmed.Slice(0, trimmed.Length - characters);

                if (part.AllUpper())
                {
                    // seems like an abbreviation in past tense (such as MEFed), so do not report
                    return false;
                }
            }
            else if (IsWellKnownWord(trimmed))
            {
                return false;
            }

            return true;
        }

        private static bool IsWellKnownWord(in ReadOnlySpan<char> text) => text.EqualsAny(WellKnownWords);

        private static string[] FindSingleWords(in ReadOnlySpan<char> text)
        {
            HashSet<string> words = null;

            foreach (ReadOnlySpan<char> word in text.WordsAsSpan(WordBoundary.WhiteSpaces))
            {
                if (word.EqualsAny(SingleWords))
                {
                    if (words is null)
                    {
                        words = new HashSet<string>();
                    }

                    words.Add(word.ToString());
                }
            }

            return words?.ToArray() ?? Array.Empty<string>();
        }

        private static new Location CreateLocation(in SyntaxToken token, in int offsetStart, in int offsetEnd)
        {
            var spanStart = token.SpanStart;

            return Analyzer.CreateLocation(token, spanStart + offsetStart, spanStart + offsetEnd);
        }

        private IEnumerable<Diagnostic> AnalyzeComment(in SyntaxToken tokenText, string text) => Enumerable.Empty<Diagnostic>()
                                                                                                           .Concat(AnalyzeCommentForStringEmptyTexts(tokenText, text)) // run over the string.Empty texts here before all others, to get the correct text as replacement text in the message
                                                                                                           .Concat(AnalyzeCommentForCompoundWords(tokenText, text))
                                                                                                           .Concat(AnalyzeCommentForSingleWords(tokenText, text));

        private IEnumerable<Diagnostic> AnalyzeCommentForStringEmptyTexts(SyntaxToken token, string tokenText)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var stringEmpty in StringEmptyTexts)
            {
                var index = tokenText.IndexOf(stringEmpty, StringComparison.Ordinal);

                if (index != -1)
                {
                    // we found a special text, so report that
                    var location = CreateLocation(token, index, index + stringEmpty.Length);

                    yield return Issue("String.Empty", location, CreateReplacementProposal(stringEmpty, "String.Empty")); // we use the side effect here that the name is the argument zero and gets used in the issue's message
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeCommentForCompoundWords(SyntaxToken token, string tokenText)
        {
            var textLength = tokenText.Length - 1;

            var start = 0;
            var end = -1;

            // loop over complete text
            while (end < textLength)
            {
                if (TryFindCompoundWord(tokenText.AsSpan(), ref start, ref end))
                {
                    // get rid of leading or trailing additional characters such as braces or sentence markers
                    var span = tokenText.AsSpan(start, end - start);
                    var trimmedStart = span.TrimStart(TrimChars.AsSpan());
                    var trimmedEnd = trimmedStart.Contains('(') && span.EndsWith(')')
                                     ? span
                                     : span.TrimEnd(TrimChars.AsSpan());

                    start += span.Length - trimmedStart.Length;
                    end -= span.Length - trimmedEnd.Length;

                    if (start > end)
                    {
                        // we are at the end, so nothing more to report
                        break;
                    }

                    var trimmed = tokenText.AsSpan(start, end - start);

                    if (IsCompoundWord(trimmed))
                    {
                        if (trimmed.StartsWith("default(") && trimmed.EndsWith(')') is false)
                        {
                            // adjust the default to include the brace as it had been trimmed above
                            var i = tokenText.IndexOf(')');

                            if (i != -1)
                            {
                                end = i + 1;
                            }
                        }

                        // we found a compound word, so report that
                        var location = CreateLocation(token, start, end);
                        var compoundWord = trimmed.ToString();

                        yield return Issue(location, CreateReplacementProposal(compoundWord, compoundWord));
                    }
                }

                // jump over word
                start = end;
            }
        }

        private IEnumerable<Diagnostic> AnalyzeCommentForSingleWords(SyntaxToken token, string tokenText)
        {
            var words = FindSingleWords(tokenText.AsSpan());

            if (words.Length <= 0)
            {
                yield break;
            }

            foreach (var word in words)
            {
                var wordIsString = word is "string";

                var startOffset = 0;

                const string EmptyStringSample = @" string ("""")";

                if (wordIsString && tokenText.StartsWith(EmptyStringSample, StringComparison.Ordinal))
                {
                    // jump over the special ' string'
                    startOffset = EmptyStringSample.Length;
                }

                // we have to be aware that the word can be part within other words, so we have to surround them with delimiters or something similar to avoid incorrect replacements
                var locations = GetAllLocations(token, word.WithDelimiters(), startOffset: startOffset + 1, endOffset: 1);

                if (locations.Count is 0)
                {
                    // we have to be aware that the word can be part within other words, so we have to surround them with delimiters or something similar to avoid incorrect replacements
                    var lastLocation = GetLastLocation(token, word, startOffset: startOffset);

                    if (lastLocation != null)
                    {
                        yield return Issue(lastLocation, CreateReplacementProposal(word, word));
                    }
                }
                else
                {
                    foreach (var location in locations)
                    {
                        if (wordIsString)
                        {
                            const string Empty = "empty ";
                            const string EmptyString = Empty + "string";

                            if (location.SourceSpan.Start > Empty.Length && location.GetText(-Empty.Length, EmptyString.Length) is EmptyString)
                            {
                                // do not report because it gets reported by MiKo_2241
                                continue;
                            }
                        }

                        yield return Issue(location, CreateReplacementProposal(word, word));
                    }
                }
            }
        }
    }
}