﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

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

        private const string StartWithArticleA = "A ";
        private const string StartWithArticleAn = "An ";
        private const string StartWithArticleThe = "The ";
        private const string StartWithArticleLowerCaseA = "a ";
        private const string StartWithArticleLowerCaseAn = "an ";
        private const string StartWithArticleLowerCaseThe = "the ";
        private const string StartWithParenthesis = "(";

        private static readonly string[] StartPhraseParts = Constants.Comments.BooleanParameterStartingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string StartPhraseParts0 = StartPhraseParts[0];
        private static readonly string StartPhraseParts1 = StartPhraseParts[1];
        private static readonly string[] EndPhraseParts = Constants.Comments.BooleanParameterEndingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string EndPhraseParts0 = EndPhraseParts[0];
        private static readonly string EndPhraseParts1 = EndPhraseParts[1];

        private static readonly string[] Conditionals = { "if", "when", "in case", "whether or not", "whether" };
        private static readonly string[] ElseConditionals = { "else", "otherwise" };

        private static readonly IComparer<string> ArticleStartComparer = new StringStartComparer(
                                                                                             StartWithArticleA,
                                                                                             StartWithArticleAn,
                                                                                             StartWithArticleThe,
                                                                                             StartWithArticleLowerCaseA,
                                                                                             StartWithArticleLowerCaseAn,
                                                                                             StartWithArticleLowerCaseThe,
                                                                                             StartWithParenthesis);

        private static readonly Pair OtherwisePair = new Pair(". Otherwise", "; otherwise");

        private static readonly string[] OtherwisePairKey = { OtherwisePair.Key };
        private static readonly Pair[] OtherwisePairArray = { OtherwisePair };

        private static readonly Lazy<MapData> MappedData = new Lazy<MapData>();

#if !NCRUNCH // do not define a static ctor to speed up tests in NCrunch
        static MiKo_2023_CodeFixProvider() => LoadData(); // ensure that we have the object available
#endif

        public override string FixableDiagnosticId => "MiKo_2023";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

//// ncrunch: no coverage end
//// ncrunch: rdi default

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, in int index, Diagnostic issue)
        {
            // fix ". Otherwise ..." comments so that they will not get split
            var mergedComment = Comment(comment, OtherwisePairKey, OtherwisePairArray);

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
                    return FixEmptyComment(comment.WithContent(XmlText(string.Empty)));

                case 1 when contents[0] is XmlTextSyntax t:
                {
                    var text = t.GetTextTrimmed().AsSpan();

                    if (text.IsEmpty)
                    {
                        return FixEmptyComment(comment);
                    }

                    // determine whether we have a comment like:
                    //    true: some condition
                    //    false: some other condition'
                    var replacement = text.Contains(':') ? ReplacementTo : Replacement;

                    var data = FindMatchingReplacementMapKeys(text);
                    var uniqueKeys = data.UniqueKeys;

//// ncrunch: no coverage start
                    for (int index = 0, length = uniqueKeys.Length; index < length; index++)
                    {
                        var key = uniqueKeys[index];

                        if (text.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                        {
//// ncrunch: no coverage end
                            var subText = text.Slice(key.Length)
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
            var mappedDataValue = MappedData.Value;

            var preparedComment = PrepareComment(comment);
            var preparedComment2 = Comment(preparedComment, mappedDataValue.ReplacementMapKeysForOthers, mappedDataValue.ReplacementMapForOthers);
            var preparedComment3 = ModifyElseOtherwisePart(preparedComment2);

            return FixComment(preparedComment3, mappedDataValue.ReplacementMapKeysForOthers, mappedDataValue.ReplacementMapForOthers);
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

            var prepared = comment.ReplaceNode(originalText, XmlText(string.Empty));

            return FixComment(prepared, info.Keys, info.Map, finalCommentContinuation);
        }

        private static XmlElementSyntax FixComment(XmlElementSyntax prepared, in ReadOnlySpan<string> replacementMapKeys, in ReadOnlySpan<Pair> replacementMap, string commentContinue = null)
        {
            var startFixed = CommentStartingWith(prepared, StartPhraseParts0, SeeLangword_True(), commentContinue ?? StartPhraseParts1);
            var bothFixed = CommentEndingWith(startFixed, EndPhraseParts0, SeeLangword_False(), EndPhraseParts1);

            var fixedComment = Comment(bothFixed, replacementMapKeys, replacementMap);

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
            var data = MappedData.Value;

            if (text.Length > 0)
            {
                if (text.StartsWith(StartWithArticleA))
                {
                    return new ConcreteMapInfo(data.ReplacementMapForA, data.ReplacementMapKeysForA, data.UniqueReplacementMapKeysForA);
                }

                if (text.StartsWith(StartWithArticleAn))
                {
                    return new ConcreteMapInfo(data.ReplacementMapForAn, data.ReplacementMapKeysForAn, data.UniqueReplacementMapKeysForAn);
                }

                if (text.StartsWith(StartWithArticleThe))
                {
                    return new ConcreteMapInfo(data.ReplacementMapForThe, data.ReplacementMapKeysForThe, data.UniqueReplacementMapKeysForThe);
                }

                if (text.StartsWith(StartWithParenthesis))
                {
                    return new ConcreteMapInfo(data.ReplacementMapForParenthesis, data.ReplacementMapKeysForParenthesis, data.UniqueReplacementMapKeysForParenthesis);
                }

                if (text.StartsWith(StartWithArticleLowerCaseA))
                {
                    return new ConcreteMapInfo(data.ReplacementMapForLowerCaseA, data.ReplacementMapKeysForLowerCaseA, data.UniqueReplacementMapKeysForA);
                }

                if (text.StartsWith(StartWithArticleLowerCaseAn))
                {
                    return new ConcreteMapInfo(data.ReplacementMapForLowerCaseAn, data.ReplacementMapKeysForLowerCaseAn, data.UniqueReplacementMapKeysForAn);
                }

                if (text.StartsWith(StartWithArticleLowerCaseThe))
                {
                    return new ConcreteMapInfo(data.ReplacementMapForLowerCaseThe, data.ReplacementMapKeysForLowerCaseThe, data.UniqueReplacementMapKeysForThe);
                }
            }

            return new ConcreteMapInfo(data.ReplacementMapForOthers, data.ReplacementMapKeysForOthers, data.UniqueReplacementMapKeysForOthers);
        }

        private readonly ref struct ConcreteMapInfo
        {
            public ConcreteMapInfo(Pair[] map, string[] keys, string[] uniqueKeys)
            {
                Map = map;
                Keys = keys;
                UniqueKeys = uniqueKeys;
            }

            public Pair[] Map { get; }

            public string[] Keys { get; }

            public string[] UniqueKeys { get; }
        }

        private sealed class MapData
        {
#pragma warning disable SA1401 // Fields should be private
            public readonly Pair[] ReplacementMapForA;
            public readonly Pair[] ReplacementMapForAn;
            public readonly Pair[] ReplacementMapForThe;
            public readonly Pair[] ReplacementMapForParenthesis;
            public readonly Pair[] ReplacementMapForOthers;
            public readonly Pair[] ReplacementMapForLowerCaseA;
            public readonly Pair[] ReplacementMapForLowerCaseAn;
            public readonly Pair[] ReplacementMapForLowerCaseThe;
            public readonly string[] ReplacementMapKeysForA;
            public readonly string[] ReplacementMapKeysForAn;
            public readonly string[] ReplacementMapKeysForThe;
            public readonly string[] ReplacementMapKeysForParenthesis;
            public readonly string[] ReplacementMapKeysForOthers;
            public readonly string[] UniqueReplacementMapKeysForA;
            public readonly string[] UniqueReplacementMapKeysForAn;
            public readonly string[] UniqueReplacementMapKeysForThe;
            public readonly string[] UniqueReplacementMapKeysForParenthesis;
            public readonly string[] UniqueReplacementMapKeysForOthers;
#pragma warning restore SA1401 // Fields should be private

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
                                                   //// new Pair(". otherwise.", OtherwiseReplacement),
                                                   //// new Pair(",otherwise", "; otherwise,"),
                                                   //// new Pair(",  otherwise ", "; otherwise, "),
                                                   //// new Pair(",  otherwise", OtherwiseReplacement),
                                                   //// new Pair("; Otherwise; ", "; "),
                                                   new Pair(OrNotPhrase + ".", "."),
                                                   new Pair(OrNotPhrase + ";", ";"),
                                                   new Pair(OrNotPhrase + ",", ","),
                                                   new Pair(". ", "; "),
                                               };

                var replacementMapKeysCommon = replacementMapCommon.ToArray(_ => _.Key);

                var replacementMap = CreateReplacementMap();
                var replacementMapKeys = replacementMap.ToArray(_ => _.Key);

                var replacementMapKeysForA = ToKeyArray(replacementMapKeys, StartWithArticleA);
                var replacementMapKeysForAn = ToKeyArray(replacementMapKeys, StartWithArticleAn);
                var replacementMapKeysForThe = ToKeyArray(replacementMapKeys, StartWithArticleThe);
                var replacementMapKeysForParenthesis = ToKeyArray(replacementMapKeys, StartWithParenthesis);
                var replacementMapKeysForLowerCaseA = ToKeyArray(replacementMapKeys, StartWithArticleLowerCaseA);
                var replacementMapKeysForLowerCaseAn = ToKeyArray(replacementMapKeys, StartWithArticleLowerCaseAn);
                var replacementMapKeysForLowerCaseThe = ToKeyArray(replacementMapKeys, StartWithArticleLowerCaseThe);

                var replacementMapKeysForAHashSet = replacementMapKeysForA.ToHashSet();
                var replacementMapKeysForAnHashSet = replacementMapKeysForAn.ToHashSet();
                var replacementMapKeysForTheHashSet = replacementMapKeysForThe.ToHashSet();
                var replacementMapKeysForLowerCaseAHashSet = replacementMapKeysForLowerCaseA.ToHashSet();
                var replacementMapKeysForLowerCaseAnHashSet = replacementMapKeysForLowerCaseAn.ToHashSet();
                var replacementMapKeysForLowerCaseTheHashSet = replacementMapKeysForLowerCaseThe.ToHashSet();
                var replacementMapKeysForParenthesisHashSet = replacementMapKeysForParenthesis.ToHashSet();
                var replacementMapKeysForOthersHashSet = replacementMapKeysCommon.ToHashSet();
                replacementMapKeysForOthersHashSet.AddRange(replacementMapKeys.ToHashSet()
                                                                              .Except(replacementMapKeysForAHashSet)
                                                                              .Except(replacementMapKeysForAnHashSet)
                                                                              .Except(replacementMapKeysForTheHashSet)
                                                                              .Except(replacementMapKeysForLowerCaseAHashSet)
                                                                              .Except(replacementMapKeysForLowerCaseAnHashSet)
                                                                              .Except(replacementMapKeysForLowerCaseTheHashSet)
                                                                              .Except(replacementMapKeysForParenthesisHashSet));

                ReplacementMapForA = ToMapArray(replacementMap, replacementMapKeysForAHashSet, replacementMapCommon);
                ReplacementMapForAn = ToMapArray(replacementMap, replacementMapKeysForAnHashSet, replacementMapCommon);
                ReplacementMapForThe = ToMapArray(replacementMap, replacementMapKeysForTheHashSet, replacementMapCommon);
                ReplacementMapForLowerCaseA = ToMapArray(replacementMap, replacementMapKeysForLowerCaseAHashSet, replacementMapCommon);
                ReplacementMapForLowerCaseAn = ToMapArray(replacementMap, replacementMapKeysForLowerCaseAnHashSet, replacementMapCommon);
                ReplacementMapForLowerCaseThe = ToMapArray(replacementMap, replacementMapKeysForLowerCaseTheHashSet, replacementMapCommon);
                ReplacementMapForParenthesis = ToMapArray(replacementMap, replacementMapKeysForParenthesisHashSet, replacementMapCommon);
                ReplacementMapForOthers = ToMapArray(replacementMap, replacementMapKeysForOthersHashSet, replacementMapCommon);

                UniqueReplacementMapKeysForA = ToUnique(replacementMapKeysForA);
                UniqueReplacementMapKeysForAn = ToUnique(replacementMapKeysForAn);
                UniqueReplacementMapKeysForThe = ToUnique(replacementMapKeysForThe);
                UniqueReplacementMapKeysForParenthesis = ToUnique(replacementMapKeysForParenthesis);
                UniqueReplacementMapKeysForOthers = ToUnique(replacementMapKeysForOthersHashSet);

                // now set keys here at the end as we want these keys sorted based on string contents (and only contain the smallest sub-sequences)
                ReplacementMapKeysForA = GetTermsForQuickLookup(UniqueReplacementMapKeysForA);
                ReplacementMapKeysForAn = GetTermsForQuickLookup(UniqueReplacementMapKeysForAn);
                ReplacementMapKeysForThe = GetTermsForQuickLookup(UniqueReplacementMapKeysForThe);
                ReplacementMapKeysForParenthesis = GetTermsForQuickLookup(UniqueReplacementMapKeysForParenthesis);
                ReplacementMapKeysForOthers = GetTermsForQuickLookup(UniqueReplacementMapKeysForOthers);

                string[] ToKeyArray(ReadOnlySpan<string> keys, string text)
                {
                    var length = keys.Length;

                    var indexInResult = 0;

                    var pool = ArrayPool<string>.Shared;
                    var rentedArray = pool.Rent(length);

                    var textSpan = text.AsSpan();

                    for (var index = 0; index < length; index++)
                    {
                        var key = keys[index];

                        if (key.AsSpan().StartsWith(textSpan))
                        {
                            rentedArray[indexInResult++] = key;
                        }
                    }

                    var results = new string[indexInResult];

                    Array.Copy(rentedArray, results, indexInResult);

                    pool.Return(rentedArray);

                    return results;
                }

                Pair[] ToMapArray(in ReadOnlySpan<Pair> map, HashSet<string> keys, Pair[] others)
                {
                    var resultIndex = 0;

                    var pool = ArrayPool<Pair>.Shared;
                    var rentedArray = pool.Rent(keys.Count + others.Length);

                    for (int index = 0, count = map.Length; index < count; index++)
                    {
                        var key = map[index];

                        if (keys.Contains(key.Key))
                        {
                            rentedArray[resultIndex++] = key;
                        }
                    }

                    others.CopyTo(rentedArray, resultIndex);

                    var results = new Pair[resultIndex + others.Length];

                    Array.Copy(rentedArray, results, results.Length);

                    pool.Return(rentedArray);

                    return results;
                }

                string[] ToUnique(IEnumerable<string> strings) => new HashSet<string>(strings, StringComparer.OrdinalIgnoreCase).ToArray();
            }

            public string[] ReplacementMapKeysForLowerCaseA => ReplacementMapKeysForA;

            public string[] ReplacementMapKeysForLowerCaseAn => ReplacementMapKeysForAn;

            public string[] ReplacementMapKeysForLowerCaseThe => ReplacementMapKeysForThe;

            private static Pair[] CreateReplacementMap()
            {
                var startTerms = CreateStartTerms();

                var textsCount = startTerms.Count;

                var index = 0;
                var replacements = new Pair[textsCount];

                foreach (var text in startTerms.OrderBy(_ => _, ArticleStartComparer).ThenByDescending(_ => _.Length).ThenBy(_ => _))
                {
                    replacements[index++] = new Pair(text, Replacement);
                }

                return replacements;
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
                                                                     .Append(' ')
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
                    var end = condition.SurroundedWith(' '); // TODO RKN: Change string creation

                    // for performance reasons we use for loops here
                    for (var verbIndex = 0; verbIndex < verbsLength; verbIndex++)
                    {
                        var verb = verbs[verbIndex];
                        var middle = " " + verb + end; // TODO RKN: Change string creation

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
                    var startTerms = new[] { "A ", "An ", "The ", " " };
                    var optionals = new[] { "Optional ", "(optional) ", "(Optional) ", "optional ", " " };

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

        private sealed class StringStartComparer : IComparer<string>
        {
            private readonly string[] m_specialOrder;

            internal StringStartComparer(params string[] specialOrder) => m_specialOrder = specialOrder;

            public int Compare(string x, string y)
            {
                var notNullX = x != null;
                var notNullY = y != null;

                if (notNullX && notNullY)
                {
                    var orders = m_specialOrder.AsSpan();

                    return GetOrder(x.AsSpan(), orders) - GetOrder(y.AsSpan(), orders);
                }

                if (notNullX)
                {
                    return 1;
                }

                if (notNullY)
                {
                    return -1;
                }

                return 0;
            }

            private static int GetOrder(in ReadOnlySpan<char> text, in ReadOnlySpan<string> orders)
            {
                for (int i = 0, length = orders.Length; i < length; i++)
                {
                    if (text.StartsWith(orders[i].AsSpan()))
                    {
                        return i;
                    }
                }

                return int.MaxValue;
            }
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}