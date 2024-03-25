using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2023_CodeFixProvider)), Shared]
    public sealed class MiKo_2023_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        private const string Replacement = " to indicate that ";
        private const string ReplacementTo = " to ";
        private const string OrNotPhrase = " or not";
        private const string OtherwiseReplacement = ";  otherwise";

//// ncrunch: rdi off
        private static readonly string[] Conditionals = { "if", "when", "in case", "whether or not", "whether" };
        private static readonly string[] ElseConditionals = { "else", "otherwise" };

        private static readonly KeyValuePair<string, string> OtherwisePair = new KeyValuePair<string, string>(". Otherwise", OtherwiseReplacement);

        private static readonly KeyValuePair<string, string>[] ReplacementMap = CreateReplacementMap(
                                                                                                 new KeyValuePair<string, string>("'true'", string.Empty),
                                                                                                 new KeyValuePair<string, string>("'True'", string.Empty),
                                                                                                 new KeyValuePair<string, string>("'TRUE'", string.Empty),
                                                                                                 new KeyValuePair<string, string>("\"true\"", string.Empty),
                                                                                                 new KeyValuePair<string, string>("\"True\"", string.Empty),
                                                                                                 new KeyValuePair<string, string>("\"TRUE\"", string.Empty),
                                                                                                 new KeyValuePair<string, string>("true", string.Empty),
                                                                                                 new KeyValuePair<string, string>("True", string.Empty),
                                                                                                 new KeyValuePair<string, string>("TRUE", string.Empty),
                                                                                                 new KeyValuePair<string, string>("'false'", string.Empty),
                                                                                                 new KeyValuePair<string, string>("'False'", string.Empty),
                                                                                                 new KeyValuePair<string, string>("'FALSE'", string.Empty),
                                                                                                 new KeyValuePair<string, string>("\"false\"", string.Empty),
                                                                                                 new KeyValuePair<string, string>("\"False\"", string.Empty),
                                                                                                 new KeyValuePair<string, string>("\"FALSE\"", string.Empty),
                                                                                                 new KeyValuePair<string, string>("false", string.Empty),
                                                                                                 new KeyValuePair<string, string>("False", string.Empty),
                                                                                                 new KeyValuePair<string, string>("FALSE", string.Empty),
                                                                                                 new KeyValuePair<string, string>("if you want to", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to in case set to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to in case ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to if given ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to when given ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to if set to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to when set to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to if ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to when ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to whether to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to set to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to given ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to . if ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to , if ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to ; if ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to : if ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to , ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to ; ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to : ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to the to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to an to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to a to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to  to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to to ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" to  ", ReplacementTo),
                                                                                                 new KeyValuePair<string, string>(" that to ", " that "),
                                                                                                 new KeyValuePair<string, string>(". otherwise.", OtherwiseReplacement),
                                                                                                 new KeyValuePair<string, string>(",  otherwise", OtherwiseReplacement),
                                                                                                 new KeyValuePair<string, string>(" otherwise; otherwise, ", "otherwise, "),
                                                                                                 new KeyValuePair<string, string>("; Otherwise; ", "; "),
                                                                                                 new KeyValuePair<string, string>(OrNotPhrase + ".", "."),
                                                                                                 new KeyValuePair<string, string>(OrNotPhrase + ";", ";"),
                                                                                                 new KeyValuePair<string, string>(OrNotPhrase + ",", ","),
                                                                                                 new KeyValuePair<string, string>(". ", "; "))
                                                                                .Distinct()
                                                                                .ToArray();

        private static readonly IReadOnlyCollection<string> ReplacementMapKeys = ReplacementMap.Select(_ => _.Key).Distinct().ToArray();
        private static readonly IReadOnlyCollection<string> ReplacementMapKeysInUpperCase = ReplacementMapKeys.Select(_ => _.ToUpperInvariant()).Distinct().ToArray();

        private static readonly string[] StartPhraseParts = Constants.Comments.BooleanParameterStartingPhraseTemplate.FormatWith('|').Split('|');
        private static readonly string[] EndPhraseParts = Constants.Comments.BooleanParameterEndingPhraseTemplate.FormatWith('|').Split('|');

//// ncrunch: rdi default

        public override string FixableDiagnosticId => MiKo_2023_BooleanParamDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2023_CodeFixTitle;

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            // fix ". Otherwise ..." comments so that they will not get split
            var mergedComment = Comment(comment, new[] { OtherwisePair.Key }, new[] { OtherwisePair });

            var firstSentence = SplitCommentAfterFirstSentence(mergedComment, out var partsAfterSentence);

            var count = partsAfterSentence.Count;

            if (count <= 0)
            {
                return FixText(firstSentence);
            }

            // fix "otherwise false" parts in 'partsAfterSequence' so that they become part of 'firstSentence'
            MoveOtherwisePartToFirstSentence(ref firstSentence, ref partsAfterSentence);

            var firstComment = FixText(firstSentence);

            if (partsAfterSentence.Any())
            {
                partsAfterSentence = partsAfterSentence.WithoutLeadingXmlComment().WithoutTrailingXmlComment();
            }

            if (partsAfterSentence.Any())
            {
                var contents = firstComment.Content.WithoutTrailingXmlComment()
                                           .Add(TrailingNewLineXmlText())
                                           .AddRange(partsAfterSentence)
                                           .Add(TrailingNewLineXmlText());

                return firstComment.WithContent(contents);
            }

            return firstComment;
        }

        private static void MoveOtherwisePartToFirstSentence(ref XmlElementSyntax firstSentence, ref SyntaxList<XmlNodeSyntax> partsAfterSentence)
        {
            var part = partsAfterSentence[0];

            if (part.IsSeeLangwordBool())
            {
                partsAfterSentence = partsAfterSentence.Remove(part);

                var firstSentenceContents = firstSentence.Content.Add(part);

                if (partsAfterSentence.FirstOrDefault() is XmlTextSyntax t)
                {
                    partsAfterSentence = partsAfterSentence.Remove(t);
                    firstSentenceContents = firstSentenceContents.Add(t);
                }

                firstSentence = firstSentence.WithContent(firstSentenceContents);
            }
        }

        private static XmlElementSyntax FixText(XmlElementSyntax comment)
        {
            var contents = comment.Content;

            if (contents.Count == 1 && contents.First() is XmlTextSyntax t)
            {
                var text = t.GetTextWithoutTrivia();

                // determine whether we have a comment like:
                //    true: some condition
                //    false: some other condition'
                var replacement = text.Contains(':') ? ReplacementTo : Replacement;

                foreach (var key in ReplacementMapKeysInUpperCase)
                {
                    if (text.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    {
                        var subText = text.Slice(key.Length)
                                          .TrimStart(Constants.TrailingSentenceMarkers)
                                          .TrimEnd(Constants.TrailingSentenceMarkers);

                        return FixTextOnlyComment(comment, t, subText, replacement);
                    }
                }

                // seems we could not fix the part
                var otherPhraseStart = text.IndexOfAny(ElseConditionals, StringComparison.OrdinalIgnoreCase);

                if (otherPhraseStart > -1)
                {
                    var subText = text.Slice(0, otherPhraseStart)
                                      .TrimStart(Constants.TrailingSentenceMarkers)
                                      .TrimEnd(Constants.TrailingSentenceMarkers);

                    return FixTextOnlyComment(comment, t, subText, replacement);
                }
            }

            var preparedComment = PrepareComment(comment);
            var preparedComment2 = Comment(preparedComment, ReplacementMapKeys, ReplacementMap);
            var preparedComment3 = ModifyElseOtherwisePart(preparedComment2);

            return FixComment(preparedComment3);
        }

        private static XmlElementSyntax FixTextOnlyComment(XmlElementSyntax comment, XmlTextSyntax originalText, ReadOnlySpan<char> subText, string replacement)
        {
            subText = ModifyOrNotPart(subText);

            foreach (var conditional in Conditionals)
            {
                if (subText.StartsWith(conditional, StringComparison.OrdinalIgnoreCase))
                {
                    subText = subText.Slice(conditional.Length).TrimStart();

                    replacement = ReplacementTo;

                    break;
                }
            }

            var prepared = comment.ReplaceNode(originalText, XmlText(string.Empty));

            var continuation = replacement == Replacement
                               ? subText.TrimStart().ToLowerCaseAt(0) // do not try to make the first word a verb as it might not be one
                               : MakeFirstWordInfiniteVerb(subText).ToString();

            var commentContinuation = new StringBuilder(replacement).Append(continuation)
                                                                    .ReplaceAllWithCheck(ReplacementMap)
                                                                    .ToString();

            return FixComment(prepared, commentContinuation);
        }

        private static XmlElementSyntax FixComment(XmlElementSyntax prepared, string commentContinue = null)
        {
            var startFixed = CommentStartingWith(prepared, StartPhraseParts[0], SeeLangword_True(), commentContinue ?? StartPhraseParts[1]);
            var bothFixed = CommentEndingWith(startFixed, EndPhraseParts[0], SeeLangword_False(), EndPhraseParts[1]);

            var fixedComment = Comment(bothFixed, ReplacementMapKeys, ReplacementMap);

            return fixedComment;
        }

        private static XmlElementSyntax ModifyElseOtherwisePart(XmlElementSyntax comment)
        {
            var followUpText = comment.Content.OfType<XmlTextSyntax>().FirstOrDefault();

            if (followUpText is null)
            {
                return comment;
            }

            var token = followUpText.TextTokens.LastOrDefault(_ => _.ValueText.ContainsAny(Constants.TrailingSentenceMarkers));

            if (token.IsDefaultValue())
            {
                // we did not find any
                return comment;
            }

            var text = token.ValueText;

            // seems we could not fix the part
            var otherPhraseStart = text.LastIndexOfAny(ElseConditionals, StringComparison.OrdinalIgnoreCase);

            if (otherPhraseStart > -1)
            {
                var subText = text.AsSpan(0, otherPhraseStart)
                                  .TrimStart(Constants.TrailingSentenceMarkers)
                                  .TrimEnd(Constants.TrailingSentenceMarkers);

                return comment.ReplaceToken(token, token.WithText(subText));
            }

            return comment;
        }

        private static ReadOnlySpan<char> ModifyOrNotPart(ReadOnlySpan<char> text)
        {
            if (text.EndsWith(OrNotPhrase, StringComparison.Ordinal))
            {
                return text.WithoutSuffix(OrNotPhrase);
            }

            return text;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            // Fix <see langword>, <b> or <c> by replacing them with nothing
            var result = RemoveBooleansTags(comment);

            // convert first word in infinite verb (if applicable)
            return MakeFirstWordInfiniteVerb(result);
        }

//// ncrunch: rdi off
        private static IEnumerable<KeyValuePair<string, string>> CreateReplacementMap(params KeyValuePair<string, string>[] additionalPairs)
        {
            var texts = CreateStartTerms().Distinct().OrderByDescending(_ => _.Length).ToList();

            foreach (var text in texts)
            {
                yield return new KeyValuePair<string, string>(text, Replacement);
            }

            foreach (var additionalPair in additionalPairs)
            {
                yield return additionalPair;
            }
        }

        private static IEnumerable<string> CreateStartTerms()
        {
            var startTerms = new[] { "A", "An", "The", string.Empty };
            var optionals = new[] { "Optional", "(optional)", "(Optional)", "optional", string.Empty };
            var booleans = new[] { "bool", "Bool", "boolean", "Boolean", string.Empty };
            var parameters = new[] { "flag", "Flag", "value", "Value", "parameter", "Parameter", "paramter", "Paramter", string.Empty };

            var starts = new List<string> { string.Empty };

            foreach (var start in from startTerm in startTerms
                                  from optional in optionals
                                  from boolean in booleans
                                  from parameter in parameters
                                  select string.Concat(startTerm, " ", optional, " ", boolean, " ", parameter))
            {
                if (start.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var fixedStart = start.Replace("   ", " ").Replace("  ", " ").Trim();
                starts.Add(fixedStart);
            }

            var conditions = new[] { "if to", "if", "whether or not to", "whether or not", "whether to", "whether" };

            var verbs = new[]
                            {
                                "controling", // be aware of typo
                                "controlling",
                                "defining",
                                "determining",
                                "determinating", // be aware of typo
                                "indicating",
                                "specifying",

                                "that controls",
                                "that defined", // be aware of typo
                                "that defines",
                                "that determined", // be aware of typo
                                "that determines",
                                "that indicated", // be aware of typo
                                "that indicates",
                                "that specified", // be aware of typo
                                "that specifies",

                                "which controls",
                                "which defined", // be aware of typo
                                "which defines",
                                "which determined", // be aware of typo
                                "which determines",
                                "which indicated", // be aware of typo
                                "which indicates",
                                "which specified", // be aware of typo
                                "which specifies",

                                "to control",
                                "to define",
                                "to determine",
                                "to indicate",
                                "to specify",
                            };

            foreach (var text in from start in starts
                                 from verb in verbs
                                 from condition in conditions
                                 select $"{start} {verb} {condition} ".TrimStart())
            {
                yield return text.ToUpperCaseAt(0);
                yield return text.ToLowerCaseAt(0);
            }

            var startingVerbs = new[] { "Controls", "Defines", "Defined", "Determines", "Determined", "Indicates", "Indicated", "Specifies", "Specified", "Controling", "Controlling", "Defining", "Determining", "Determinating", "Determing", "Indicating", "Specifying" };

            foreach (var text in from startingVerb in startingVerbs
                                 from condition in conditions
                                 select $"{startingVerb} {condition} ")
            {
                yield return text.ToUpperCaseAt(0);
                yield return text.ToLowerCaseAt(0);
            }
        }

        //// ncrunch: rdi default
    }
}