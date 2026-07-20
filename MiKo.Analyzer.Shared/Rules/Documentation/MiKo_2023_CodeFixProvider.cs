using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2023_CodeFixProvider)), Shared]
    public sealed class MiKo_2023_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
//// ncrunch: rdi off
//// ncrunch: no coverage start

        private const string Replacement = " to indicate that ";
        private const string ReplacementTo = " to ";
        private const string OrNotPhrase = " or not";
        private const string OtherwiseReplacement = "; otherwise, ";

        private static readonly string[] StartPhraseParts = Constants.Comments.BooleanParameterStartingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string StartPhraseParts0 = StartPhraseParts[0];
        private static readonly string StartPhraseParts1 = StartPhraseParts[1];
        private static readonly string[] EndPhraseParts = Constants.Comments.BooleanParameterEndingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string EndPhraseParts0 = EndPhraseParts[0];
        private static readonly string EndPhraseParts1 = EndPhraseParts[1];

        private static readonly string[] Conditionals = { "if", "when", "in case", "whether or not", "whether" };
        private static readonly string[] ElseConditionals = { "else", "otherwise" };

        private static readonly Regex ShouldBeRegex = new Regex(@"\b(shall|should|can|could|must|may|might|would)\s+be\s+\w+\b", RegexOptions.Compiled, 250.Milliseconds());

        private static readonly ReplacementMap OtherwiseMap = new ReplacementMap("MiKo_2023_Otherwise", new[] { new Pair(". Otherwise", "; otherwise") }, _ => _.ToArray(__ => __.Key));

        private static readonly Lazy<MapData> MappedData = new Lazy<MapData>();

        public override string FixableDiagnosticId => "MiKo_2023";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

//// ncrunch: no coverage end
//// ncrunch: rdi default

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, in int index, Diagnostic issue)
        {
            // fix ". Otherwise ..." comments so that they will not get split
            var mergedComment = Comment(comment, OtherwiseMap);

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
                                           .AddRange(partsAfterSentence);

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
            var count = contents.Count;

            switch (count)
            {
                case 0:
                    return FixEmptyComment(comment.WithContent(XmlText()));

                case 1 when contents[0] is XmlTextSyntax t:
                {
                    var textTrimmed = t.GetTextTrimmed();
                    var text = textTrimmed.AsSpan();

                    if (text.IsEmpty)
                    {
                        return FixEmptyComment(comment);
                    }

                    var data = FindMatchingReplacementMapKeys(text);

                    try
                    {
                        var shouldBeMatch = ShouldBeRegex.Match(textTrimmed);

                        if (shouldBeMatch.Success)
                        {
                            var shouldBeText = shouldBeMatch.Value;
                            var newTextStart = ReplacementTo + Verbalizer.MakeInfiniteVerb(shouldBeText.ThirdWord()) + Constants.SingleSpace;

                            // re-phrase the text to fix it later on
                            var updatedText = textTrimmed.AsCachedBuilder()
                                                         .ReplaceAllWithProbe(data.Map)
                                                         .Without(Constants.SingleSpace + shouldBeText)
                                                         .TrimmedStart()
                                                         .WithoutStarting(Conditionals, StringComparison.OrdinalIgnoreCase)
                                                         .TrimmedStart()
                                                         .Insert(0, newTextStart)
                                                         .ToStringAndRelease();

                            // do a second round with the updated text, to fix all the other stuff
                            return FixText(comment.WithContent(XmlText(updatedText)));
                        }
                    }
                    catch (RegexMatchTimeoutException)
                    {
                        // we were unable to detect within time, so we assume that the text was not part of it and fall through
                    }

                    // determine whether we have a comment like:
                    //    true: some condition
                    //    false: some other condition
                    var replacement = text.Contains(':') ? ReplacementTo : Replacement;

                    var uniqueKeys = data.UniqueKeys;

//// ncrunch: no coverage start
                    for (int index = 0, length = uniqueKeys.Length; index < length; index++)
                    {
                        var uniqueKey = uniqueKeys[index];

                        if (text.StartsWith(uniqueKey, StringComparison.OrdinalIgnoreCase))
                        {
//// ncrunch: no coverage end
                            var subText = text.Slice(uniqueKey.Length)
                                              .TrimStart(Constants.TrailingSentenceMarkers)
                                              .TrimEnd(Constants.TrailingSentenceMarkers);

                            return FixTextOnlyComment(comment, t, subText, replacement, data);
                        }
                    } // ncrunch: no coverage

                    // seems we could not fix the part
                    var otherPhraseStart = text.IndexOfAny(ElseConditionals, StringComparison.OrdinalIgnoreCase);

                    if (otherPhraseStart > -1)
                    {
                        var subText = text.Slice(0, otherPhraseStart)
                                          .TrimStart(Constants.TrailingSentenceMarkers)
                                          .TrimEnd(Constants.TrailingSentenceMarkers);

                        return FixTextOnlyComment(comment, t, subText, replacement, FindMatchingReplacementMapKeys(ReadOnlySpan<char>.Empty));
                    }

                    break;
                }
            }

            // now get all data
            var info = MappedData.Value.FindMatchingMapInfo(ReadOnlySpan<char>.Empty);

            var preparedComment = PrepareComment(comment);
            var preparedComment2 = Comment(preparedComment, info.Map);
            var preparedComment3 = ModifyElseOtherwisePart(preparedComment2);

            return FixComment(preparedComment3, info.Map);
        }

        private static XmlElementSyntax FixEmptyComment(XmlElementSyntax comment)
        {
            var startFixed = CommentStartingWith(comment, StartPhraseParts0, SeeLangword_True(), Replacement + Constants.TODO);
            var bothFixed = CommentEndingWith(startFixed, EndPhraseParts0, SeeLangword_False(), EndPhraseParts1);

            return bothFixed.WithTagsOnSeparateLines();
        }

        private static XmlElementSyntax FixTextOnlyComment(XmlElementSyntax comment, XmlTextSyntax originalText, in ReadOnlySpan<char> subText, string replacement, in ConcreteMapInfo info)
        {
            var text = ModifyOrNotPart(subText);

            for (int index = 0, length = Conditionals.Length; index < length; index++)
            {
                var conditional = Conditionals[index];

                if (text.StartsWith(conditional, StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Slice(conditional.Length).TrimStart();

                    replacement = ReplacementTo;

                    break;
                }
            }

            var commentContinuation = StringBuilderCache.Acquire();

            // be aware of a gerund verb
            if (replacement == ReplacementTo || Verbalizer.IsGerundVerb(text.FirstWord()))
            {
                commentContinuation.Append(ReplacementTo);

                var continuation = Verbalizer.MakeFirstWordInfiniteVerb(text, FirstWordAdjustment.StartLowerCase);

                commentContinuation.Append(continuation);
            }
            else
            {
                commentContinuation.Append(replacement);

                // do not try to make the first word a verb as it might not be one
                var continuation = text.TrimStart().ToLowerCaseAt(0);

                commentContinuation.Append(continuation);
            }

            commentContinuation.ReplaceAllWithProbe(info.Map);

            var finalCommentContinuation = StringBuilderCache.GetStringAndRelease(commentContinuation);

            var prepared = comment.ReplaceNode(originalText, XmlText());

            return FixComment(prepared, info.Map, finalCommentContinuation);
        }

        private static XmlElementSyntax FixComment(XmlElementSyntax prepared, ReplacementMap replacementMap, string commentContinue = null)
        {
            var startFixed = CommentStartingWith(prepared, StartPhraseParts0, SeeLangword_True(), commentContinue ?? StartPhraseParts1);
            var bothFixed = CommentEndingWith(startFixed, EndPhraseParts0, SeeLangword_False(), EndPhraseParts1);

            var fixedComment = Comment(bothFixed, replacementMap);

            return fixedComment.WithTagsOnSeparateLines();
        }

        private static XmlElementSyntax ModifyElseOtherwisePart(XmlElementSyntax comment)
        {
            var followUpTexts = comment.Content.OfType<XmlTextSyntax>();

            if (followUpTexts.Count <= 0)
            {
                return comment;
            }

            var followUpText = followUpTexts[0];

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

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            // Fix <see langword>, <b> or <c> by replacing them with nothing
            var result = RemoveBooleansTags(comment);

            // convert first word in infinite verb (if applicable)
            return MakeFirstWordInfiniteVerb(result);
        }

//// ncrunch: rdi off
//// ncrunch: no coverage start

        private static ReadOnlySpan<char> ModifyOrNotPart(in ReadOnlySpan<char> text) => text.WithoutSuffix(OrNotPhrase);

        private static ConcreteMapInfo FindMatchingReplacementMapKeys(in ReadOnlySpan<char> text)
        {
            // now get all data
            return MappedData.Value.FindMatchingMapInfo(text);
        }

        private readonly ref struct ConcreteMapInfo
        {
            public ConcreteMapInfo(ReplacementMap map, string[] uniqueKeys)
            {
                Map = map;
                UniqueKeys = uniqueKeys;
            }

            public ReplacementMap Map { get; }

            public string[] UniqueKeys { get; }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "I want snake-cased names.")]
        private sealed class MapData
        {
#pragma warning disable SA1306 // Field should begin with lower-case letter
#pragma warning disable SA1310 // Field should not contain underscore

            // a
            private readonly ReplacementMap ReplacementMap_for_LowerCase_A;
            private readonly ReplacementMap ReplacementMap_for_A;
            private readonly string[] Unique_ReplacementMap_keys_for_A;

            private readonly ReplacementMap ReplacementMap_for_LowerCase_A_Oo;
            private readonly ReplacementMap ReplacementMap_for_A_Oo;
            private readonly string[] Unique_ReplacementMap_keys_for_A_Oo;

            private readonly ReplacementMap ReplacementMap_for_LowerCase_A_Parenthesis;
            private readonly ReplacementMap ReplacementMap_for_A_Parenthesis;
            private readonly string[] Unique_ReplacementMap_keys_for_A_Parenthesis;

            // an
            private readonly ReplacementMap ReplacementMap_for_LowerCase_An;
            private readonly ReplacementMap ReplacementMap_for_An;
            private readonly string[] Unique_ReplacementMap_keys_for_An;

            private readonly ReplacementMap ReplacementMap_for_LowerCase_An_Oo;
            private readonly ReplacementMap ReplacementMap_for_An_Oo;
            private readonly string[] Unique_ReplacementMap_keys_for_An_Oo;

            private readonly ReplacementMap ReplacementMap_for_LowerCase_An_Parenthesis;
            private readonly ReplacementMap ReplacementMap_for_An_Parenthesis;
            private readonly string[] Unique_ReplacementMap_keys_for_An_Parenthesis;

            // the
            private readonly ReplacementMap ReplacementMap_for_LowerCase_The;
            private readonly ReplacementMap ReplacementMap_for_The;
            private readonly string[] Unique_ReplacementMap_keys_for_The;

            private readonly ReplacementMap ReplacementMap_for_LowerCase_The_Oo;
            private readonly ReplacementMap ReplacementMap_for_The_Oo;
            private readonly string[] Unique_ReplacementMap_keys_for_The_Oo;

            private readonly ReplacementMap ReplacementMap_for_LowerCase_The_Parenthesis;
            private readonly ReplacementMap ReplacementMap_for_The_Parenthesis;
            private readonly string[] Unique_ReplacementMap_keys_for_The_Parenthesis;

            // optional
            private readonly ReplacementMap ReplacementMap_for_LowerCase_Optional;
            private readonly ReplacementMap ReplacementMap_for_Optional;
            private readonly string[] Unique_ReplacementMap_keys_for_Optional;

            // others
            private readonly ReplacementMap ReplacementMap_for_Others;
            private readonly string[] Unique_ReplacementMap_keys_for_Others;

            // (
            private readonly ReplacementMap ReplacementMap_for_Parenthesis;
            private readonly string[] Unique_ReplacementMap_keys_for_Parenthesis;

#pragma warning restore SA1306 // Field should begin with lower-case letter
#pragma warning restore SA1310 // Field should not contain underscore

            public MapData()
            {
                var replacementMapCommon = new[]
                                               {
                                                   new Pair("'true'"),
                                                   new Pair("'True'"),
                                                   new Pair("'TRUE'"),
                                                   new Pair("\"true\""),
                                                   new Pair("\"True\""),
                                                   new Pair("\"TRUE\""),
                                                   new Pair("true"),
                                                   new Pair("True"),
                                                   new Pair("TRUE"),
                                                   new Pair("'false'"),
                                                   new Pair("'False'"),
                                                   new Pair("'FALSE'"),
                                                   new Pair("\"false\""),
                                                   new Pair("\"False\""),
                                                   new Pair("\"FALSE\""),
                                                   new Pair("false"),
                                                   new Pair("False"),
                                                   new Pair("FALSE"),
                                                   new Pair("; otherwise ; otherwise, ", OtherwiseReplacement),
                                                   new Pair(", otherwise ; otherwise, ", OtherwiseReplacement),
                                                   new Pair(";  otherwise; otherwise, ", OtherwiseReplacement),
                                                   new Pair(",  otherwise; otherwise, ", OtherwiseReplacement),
                                                   new Pair("; otherwise; otherwise, ", OtherwiseReplacement),
                                                   new Pair(", otherwise; otherwise, ", OtherwiseReplacement),
                                                   new Pair("if you want to", ReplacementTo),
                                                   new Pair("if you need ", ReplacementTo),
                                                   new Pair("if this is ", ReplacementTo),
                                                   new Pair(" to value indicating, whether ", Replacement),
                                                   new Pair(" to value indicating, that ", Replacement),
                                                   new Pair(" to value indicating, if ", Replacement),
                                                   new Pair(" to value indicating whether ", Replacement),
                                                   new Pair(" to value indicating that ", Replacement),
                                                   new Pair(" to value indicating if ", Replacement),
                                                   new Pair(" to in case set to ", ReplacementTo),
                                                   new Pair(" to in case ", ReplacementTo),
                                                   new Pair(" to if given ", ReplacementTo),
                                                   new Pair(" to when given ", ReplacementTo),
                                                   new Pair(" to if set to ", ReplacementTo),
                                                   new Pair(" to when set to ", ReplacementTo),
                                                   new Pair(" to if ", ReplacementTo),
                                                   new Pair(" to when ", ReplacementTo),
                                                   new Pair(" to whether to ", ReplacementTo),
                                                   new Pair(" to set to ", ReplacementTo),
                                                   new Pair(" to given ", ReplacementTo),
                                                   new Pair(" to use  if ", Replacement),
                                                   new Pair(" to use  when ", Replacement),
                                                   new Pair(" to . if ", ReplacementTo),
                                                   new Pair(" to , if ", ReplacementTo),
                                                   new Pair(" to ; if ", ReplacementTo),
                                                   new Pair(" to : if ", ReplacementTo),
                                                   new Pair(" to , ", ReplacementTo),
                                                   new Pair(" to ; ", ReplacementTo),
                                                   new Pair(" to : ", ReplacementTo),
                                                   new Pair(" to the to ", ReplacementTo),
                                                   new Pair(" to an to ", ReplacementTo),
                                                   new Pair(" to a to ", ReplacementTo),
                                                   new Pair(" to  to ", ReplacementTo),
                                                   new Pair(" to to ", ReplacementTo),
                                                   new Pair(" to  ", ReplacementTo),
                                                   new Pair(" to the ", Replacement + "the "),
                                                   new Pair(" to an ", Replacement + "an "),
                                                   new Pair(" to a ", Replacement + "a "),
                                                   new Pair(" that to ", " that "),
                                                   new Pair(" the the ", " the "),
                                                   new Pair(" an an ", " an "),
                                                   new Pair(" a a ", " a "),
                                                   new Pair(" to indicate that indicate that ", Replacement),
                                                   new Pair("; otherwise,; otherwise, ", OtherwiseReplacement),
                                                   //// new Pair(". otherwise.", OtherwiseReplacement),
                                                   //// new Pair(",otherwise", OtherwiseReplacement),
                                                   //// new Pair(",  otherwise ", OtherwiseReplacement),
                                                   //// new Pair(",  otherwise", OtherwiseReplacement),
                                                   //// new Pair("; Otherwise; ", "; "),
                                                   new Pair(OrNotPhrase + ".", "."),
                                                   new Pair(OrNotPhrase + ";", ";"),
                                                   new Pair(OrNotPhrase + ",", ","),
                                                   new Pair(". ", "; "),
                                               };

                var replacementMapKeysForParenthesis = new HashSet<string>();

                var replacementMapKeysForA = new HashSet<string>();
                var replacementMapKeysForA_Parenthesis = new HashSet<string>();
                var replacementMapKeysForA_Oo = new HashSet<string>();

                var replacementMapKeysForAn = new HashSet<string>();
                var replacementMapKeysForAn_Parenthesis = new HashSet<string>();
                var replacementMapKeysForAn_Oo = new HashSet<string>();

                var replacementMapKeysForThe = new HashSet<string>();
                var replacementMapKeysForThe_Parenthesis = new HashSet<string>();
                var replacementMapKeysForThe_Oo = new HashSet<string>();

                var replacementMapKeysForOptional = new HashSet<string>();
                var replacementMapKeysForOthers = new HashSet<string>();

                foreach (var start in CreateStartTerms())
                {
                    var hashSet = GetCorrectHashSet(start);

                    hashSet.Add(start);
                }

                // remove the lower case stuff and return them as an own hash set
                var replacementMapKeysForLowerCaseA = SplitOutLowerCaseHashSet(replacementMapKeysForA);
                var replacementMapKeysForLowerCaseA_Parenthesis = SplitOutLowerCaseHashSet(replacementMapKeysForA_Parenthesis);
                var replacementMapKeysForLowerCaseA_Oo = SplitOutLowerCaseHashSet(replacementMapKeysForA_Oo);

                var replacementMapKeysForLowerCaseAn = SplitOutLowerCaseHashSet(replacementMapKeysForAn);
                var replacementMapKeysForLowerCaseAn_Parenthesis = SplitOutLowerCaseHashSet(replacementMapKeysForAn_Parenthesis);
                var replacementMapKeysForLowerCaseAn_Oo = SplitOutLowerCaseHashSet(replacementMapKeysForAn_Oo);

                var replacementMapKeysForLowerCaseThe = SplitOutLowerCaseHashSet(replacementMapKeysForThe);
                var replacementMapKeysForLowerCaseThe_Parenthesis = SplitOutLowerCaseHashSet(replacementMapKeysForThe_Parenthesis);
                var replacementMapKeysForLowerCaseThe_Oo = SplitOutLowerCaseHashSet(replacementMapKeysForThe_Oo);

                var replacementMapKeysForLowerCaseOptional = SplitOutLowerCaseHashSet(replacementMapKeysForOptional);

                // split the code into multiple lists
                ReplacementMap_for_A = ToMap("MiKo_2023_A", replacementMapKeysForA, replacementMapCommon);
                ReplacementMap_for_LowerCase_A = ToMap("MiKo_2023_a", replacementMapKeysForLowerCaseA, replacementMapCommon);
                ReplacementMap_for_An = ToMap("MiKo_2023_An", replacementMapKeysForAn, replacementMapCommon);
                ReplacementMap_for_LowerCase_An = ToMap("MiKo_2023_an", replacementMapKeysForLowerCaseAn, replacementMapCommon);
                ReplacementMap_for_The = ToMap("MiKo_2023_The", replacementMapKeysForThe, replacementMapCommon);
                ReplacementMap_for_LowerCase_The = ToMap("MiKo_2023_the", replacementMapKeysForLowerCaseThe, replacementMapCommon);
                ReplacementMap_for_A_Parenthesis = ToMap("MiKo_2023_A_(", replacementMapKeysForA_Parenthesis, replacementMapCommon);
                ReplacementMap_for_LowerCase_A_Parenthesis = ToMap("MiKo_2023_a_(", replacementMapKeysForLowerCaseA_Parenthesis, replacementMapCommon);
                ReplacementMap_for_An_Parenthesis = ToMap("MiKo_2023_An_(", replacementMapKeysForAn_Parenthesis, replacementMapCommon);
                ReplacementMap_for_LowerCase_An_Parenthesis = ToMap("MiKo_2023_an_(", replacementMapKeysForLowerCaseAn_Parenthesis, replacementMapCommon);
                ReplacementMap_for_The_Parenthesis = ToMap("MiKo_2023_The_(", replacementMapKeysForThe_Parenthesis, replacementMapCommon);
                ReplacementMap_for_LowerCase_The_Parenthesis = ToMap("MiKo_2023_the_(", replacementMapKeysForLowerCaseThe_Parenthesis, replacementMapCommon);
                ReplacementMap_for_A_Oo = ToMap("MiKo_2023_A_Oo", replacementMapKeysForA_Oo, replacementMapCommon);
                ReplacementMap_for_LowerCase_A_Oo = ToMap("MiKo_2023_a_Oo", replacementMapKeysForLowerCaseA_Oo, replacementMapCommon);
                ReplacementMap_for_An_Oo = ToMap("MiKo_2023_An_Oo", replacementMapKeysForAn_Oo, replacementMapCommon);
                ReplacementMap_for_LowerCase_An_Oo = ToMap("MiKo_2023_an_Oo", replacementMapKeysForLowerCaseAn_Oo, replacementMapCommon);
                ReplacementMap_for_The_Oo = ToMap("MiKo_2023_The_Oo", replacementMapKeysForThe_Oo, replacementMapCommon);
                ReplacementMap_for_LowerCase_The_Oo = ToMap("MiKo_2023_the_Oo", replacementMapKeysForLowerCaseThe_Oo, replacementMapCommon);

                ReplacementMap_for_Optional = ToMap("MiKo_2023_Optional", replacementMapKeysForOptional, replacementMapCommon);
                ReplacementMap_for_LowerCase_Optional = ToMap("MiKo_2023_optional", replacementMapKeysForLowerCaseOptional, replacementMapCommon);

                ReplacementMap_for_Parenthesis = ToMap("MiKo_2023_(", replacementMapKeysForParenthesis, replacementMapCommon);
                ReplacementMap_for_Others = ToMap("MiKo_2023_others", replacementMapKeysForOthers, replacementMapCommon);

                Unique_ReplacementMap_keys_for_A = ToUnique(ReplacementMap_for_A);
                Unique_ReplacementMap_keys_for_An = ToUnique(ReplacementMap_for_An);
                Unique_ReplacementMap_keys_for_The = ToUnique(ReplacementMap_for_The);
                Unique_ReplacementMap_keys_for_A_Parenthesis = ToUnique(ReplacementMap_for_A_Parenthesis);
                Unique_ReplacementMap_keys_for_An_Parenthesis = ToUnique(ReplacementMap_for_An_Parenthesis);
                Unique_ReplacementMap_keys_for_The_Parenthesis = ToUnique(ReplacementMap_for_The_Parenthesis);
                Unique_ReplacementMap_keys_for_A_Oo = ToUnique(ReplacementMap_for_A_Oo);
                Unique_ReplacementMap_keys_for_An_Oo = ToUnique(ReplacementMap_for_An_Oo);
                Unique_ReplacementMap_keys_for_The_Oo = ToUnique(ReplacementMap_for_The_Oo);
                Unique_ReplacementMap_keys_for_Optional = ToUnique(ReplacementMap_for_Optional);
                Unique_ReplacementMap_keys_for_Parenthesis = ToUnique(ReplacementMap_for_Parenthesis);
                Unique_ReplacementMap_keys_for_Others = ToUnique(ReplacementMap_for_Others);

                // now set keys here at the end as we want these keys sorted based on string contents (and only contain the smallest sub-sequences)
                ReplacementMap_for_A.Keys = ReplacementMap_for_LowerCase_A.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_A);
                ReplacementMap_for_An.Keys = ReplacementMap_for_LowerCase_An.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_An);
                ReplacementMap_for_The.Keys = ReplacementMap_for_LowerCase_The.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_The);
                ReplacementMap_for_A_Parenthesis.Keys = ReplacementMap_for_LowerCase_A_Parenthesis.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_A_Parenthesis);
                ReplacementMap_for_An_Parenthesis.Keys = ReplacementMap_for_LowerCase_An_Parenthesis.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_An_Parenthesis);
                ReplacementMap_for_The_Parenthesis.Keys = ReplacementMap_for_LowerCase_The_Parenthesis.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_The_Parenthesis);
                ReplacementMap_for_A_Oo.Keys = ReplacementMap_for_LowerCase_A_Oo.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_A_Oo);
                ReplacementMap_for_An_Oo.Keys = ReplacementMap_for_LowerCase_An_Oo.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_An_Oo);
                ReplacementMap_for_The_Oo.Keys = ReplacementMap_for_LowerCase_The_Oo.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_The_Oo);
                ReplacementMap_for_Optional.Keys = ReplacementMap_for_LowerCase_Optional.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_Optional);
                ReplacementMap_for_Parenthesis.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_Parenthesis);
                ReplacementMap_for_Others.Keys = GetTermsForQuickLookup(Unique_ReplacementMap_keys_for_Others);

                return;

                ReplacementMap ToMap(string id, HashSet<string> keys, Pair[] others)
                {
                    var sorted = keys.OrderDescendingByLengthAndText();
                    var results = new Pair[sorted.Length + others.Length];

                    for (int index = 0, count = sorted.Length; index < count; index++)
                    {
                        results[index] = new Pair(sorted[index]);
                    }

                    Array.Copy(others, 0, results, sorted.Length, others.Length);

                    return new ReplacementMap(id, results, Array.Empty<string>()); // keys will be set later on
                }

                string[] ToUnique(ReplacementMap map) => new HashSet<string>(map.Pairs.Select(_ => _.Key), StringComparer.OrdinalIgnoreCase).ToArray();

                HashSet<string> SplitOutLowerCaseHashSet(HashSet<string> hashSet)
                {
                    var lowerHashSet = new HashSet<string>(hashSet.Where(_ => _[0].IsLowerCase()));

                    hashSet.ExceptWith(lowerHashSet);

                    return lowerHashSet;
                }

                HashSet<string> GetCorrectHashSet(string text)
                {
                    if (text.StartsWith('('))
                    {
                        return replacementMapKeysForParenthesis;
                    }

                    if (text.StartsWith("A ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (text.Length > 2)
                        {
                            switch (text[2])
                            {
                                case '(':
                                    return replacementMapKeysForA_Parenthesis;
                                case 'O':
                                case 'o':
                                    return replacementMapKeysForA_Oo;
                            }
                        }

                        return replacementMapKeysForA;
                    }

                    if (text.StartsWith("An ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (text.Length > 3)
                        {
                            switch (text[3])
                            {
                                case '(':
                                    return replacementMapKeysForAn_Parenthesis;
                                case 'O':
                                case 'o':
                                    return replacementMapKeysForAn_Oo;
                            }
                        }

                        return replacementMapKeysForAn;
                    }

                    if (text.StartsWith("The ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (text.Length > 4)
                        {
                            switch (text[4])
                            {
                                case '(':
                                    return replacementMapKeysForThe_Parenthesis;
                                case 'O':
                                case 'o':
                                    return replacementMapKeysForThe_Oo;
                            }
                        }

                        return replacementMapKeysForThe;
                    }

                    if (text.StartsWith("Optional ", StringComparison.OrdinalIgnoreCase))
                    {
                        return replacementMapKeysForOptional;
                    }

                    return replacementMapKeysForOthers;
                }
            }

            public ConcreteMapInfo FindMatchingMapInfo(in ReadOnlySpan<char> text)
            {
                if (text.StartsWith("(", StringComparison.Ordinal))
                {
                    return new ConcreteMapInfo(ReplacementMap_for_Parenthesis, Unique_ReplacementMap_keys_for_Parenthesis);
                }

                if (text.StartsWith("A ", StringComparison.OrdinalIgnoreCase))
                {
                    var lowerCase = text[0] is 'a';

                    if (text.Length > 2)
                    {
                        switch (text[2])
                        {
                            case '(':
                                return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_A_Parenthesis : ReplacementMap_for_A_Parenthesis, Unique_ReplacementMap_keys_for_A_Parenthesis);
                            case 'O':
                            case 'o':
                                return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_A_Oo : ReplacementMap_for_A_Oo, Unique_ReplacementMap_keys_for_A_Oo);
                        }
                    }

                    return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_A : ReplacementMap_for_A, Unique_ReplacementMap_keys_for_A);
                }

                if (text.StartsWith("An ", StringComparison.OrdinalIgnoreCase))
                {
                    var lowerCase = text[0] is 'a';

                    if (text.Length > 3)
                    {
                        switch (text[3])
                        {
                            case '(':
                                return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_An_Parenthesis : ReplacementMap_for_An_Parenthesis, Unique_ReplacementMap_keys_for_An_Parenthesis);
                            case 'O':
                            case 'o':
                                return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_An_Oo : ReplacementMap_for_An_Oo, Unique_ReplacementMap_keys_for_An_Oo);
                        }
                    }

                    return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_An : ReplacementMap_for_An, Unique_ReplacementMap_keys_for_An);
                }

                if (text.StartsWith("The ", StringComparison.OrdinalIgnoreCase))
                {
                    var lowerCase = text[0] is 't';

                    if (text.Length > 4)
                    {
                        switch (text[4])
                        {
                            case '(':
                                return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_The_Parenthesis : ReplacementMap_for_The_Parenthesis, Unique_ReplacementMap_keys_for_The_Parenthesis);
                            case 'O':
                            case 'o':
                                return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_The_Oo : ReplacementMap_for_The_Oo, Unique_ReplacementMap_keys_for_The_Oo);
                        }
                    }

                    return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_The : ReplacementMap_for_The, Unique_ReplacementMap_keys_for_The);
                }

                if (text.StartsWith("Optional ", StringComparison.OrdinalIgnoreCase))
                {
                    var lowerCase = text[0] is 'o';

                    return new ConcreteMapInfo(lowerCase ? ReplacementMap_for_LowerCase_Optional : ReplacementMap_for_Optional, Unique_ReplacementMap_keys_for_Optional);
                }

                return new ConcreteMapInfo(ReplacementMap_for_Others, Unique_ReplacementMap_keys_for_Others);
            }

            private static HashSet<string> CreateStartTerms()
            {
                var optionalStarts = CreateStartTermsWithOptionals();

                var optionalStartsLength = optionalStarts.Length;

                var booleans = new[] { "bool", "Bool", "boolean", "Boolean", string.Empty };
                var parameters = new[] { "flag", "Flag", "value", "Value", "parameter", "Parameter", "paramter", "Paramter", string.Empty };
                var booleansLength = booleans.Length;
                var parametersLength = parameters.Length;

                var starts = new List<string>((optionalStartsLength * booleansLength * parametersLength) + 1) { string.Empty };

                for (var optionalIndex = 0; optionalIndex < optionalStartsLength; optionalIndex++)
                {
                    var optionalStart = optionalStarts[optionalIndex];

                    for (var booleanIndex = 0; booleanIndex < booleansLength; booleanIndex++)
                    {
                        // we have lots of loops, so cache data to avoid unnecessary calculations
                        var boolean = booleans[booleanIndex];

                        var optionalBooleanStart = StringBuilderCache.Acquire(optionalStart.Length + boolean.Length + 1)
                                                                     .Append(optionalStart)
                                                                     .Append(boolean)
                                                                     .Append(Constants.Space)
                                                                     .WithoutMultipleWhiteSpaces()
                                                                     .TrimmedStart()
                                                                     .ToStringAndRelease();

                        for (var parameterIndex = 0; parameterIndex < parametersLength; parameterIndex++)
                        {
                            var fixedStart = optionalBooleanStart.AsCachedBuilder().Append(parameters[parameterIndex]).TrimmedEnd().ToStringAndRelease();

                            if (fixedStart.IsNullOrWhiteSpace() is false)
                            {
                                starts.Add(fixedStart);
                            }
                        }
                    }
                }

                // ignore the 'A', 'An' and 'The'-only texts without further values as this is unlikely to see in production source code
                starts.Remove("A");
                starts.Remove("An");
                starts.Remove("The");

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

                var startingVerbs = new[] { "Controls", "Defines", "Defined", "Determines", "Determined", "Indicates", "Indicated", "Specifies", "Specified", "Controling", "Controlling", "Defining", "Determining", "Determinating", "Determing", "Indicating", "Specifying" };

                var verbsLength = verbs.Length;
                var startsCount = starts.Count;
                var startingVerbsLength = startingVerbs.Length;
                var conditionsLength = conditions.Length;

                var results = new HashSet<string>();

                // for performance reasons we use for loops here
                for (var conditionIndex = 0; conditionIndex < conditionsLength; conditionIndex++)
                {
                    var condition = conditions[conditionIndex];

                    // we have lots of loops, so cache data to avoid unnecessary calculations
                    var end = condition.SurroundedWith(Constants.Space); // TODO RKN: Change string creation

                    // for performance reasons we use for loops here
                    for (var verbIndex = 0; verbIndex < verbsLength; verbIndex++)
                    {
                        var verb = verbs[verbIndex];
                        var middle = Constants.SingleSpace + verb + end; // TODO RKN: Change string creation

                        // for performance reasons we use for loops here
                        for (var startIndex = 0; startIndex < startsCount; startIndex++)
                        {
                            var text = starts[startIndex].AsCachedBuilder().Append(middle).TrimmedStart().ToStringAndRelease();

                            results.Add(text.ToUpperCaseAt(0));
                            results.Add(text.ToLowerCaseAt(0));
                        }
                    }

                    // for performance reasons we use for loops here
                    for (var index = 0; index < startingVerbsLength; index++)
                    {
                        var startingVerb = startingVerbs[index];
                        var text = startingVerb + end; // TODO RKN: Change string creation

                        results.Add(text.ToUpperCaseAt(0));
                        results.Add(text.ToLowerCaseAt(0));
                    }
                }

                return results;

                string[] CreateStartTermsWithOptionals()
                {
                    var startTerms = new[] { "A ", "An ", "The ", Constants.SingleSpace };
                    var optionals = new[] { "Optional ", "(optional) ", "(Optional) ", "optional ", Constants.SingleSpace };

                    var startTermsLength = startTerms.Length;
                    var optionalsLength = optionals.Length;

                    var index = 0;
                    var strings = new string[startTermsLength * optionalsLength];

                    // we have lots of loops, so cache data to avoid unnecessary calculations
                    for (var startTermIndex = 0; startTermIndex < startTermsLength; startTermIndex++)
                    {
                        var startTerm = startTerms[startTermIndex];

                        for (var optionalIndex = 0; optionalIndex < optionalsLength; optionalIndex++)
                        {
                            var optional = optionals[optionalIndex];

                            strings[index++] = startTerm + optional;
                        }
                    }

                    return strings;
                }
            }
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}