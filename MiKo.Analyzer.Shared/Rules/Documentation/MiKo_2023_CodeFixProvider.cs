using System;
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

        private static readonly string[] StartPhraseParts = Constants.Comments.BooleanParameterStartingPhraseTemplate.FormatWith('|').Split('|');
        private static readonly string StartPhraseParts0 = StartPhraseParts[0];
        private static readonly string StartPhraseParts1 = StartPhraseParts[1];
        private static readonly string[] EndPhraseParts = Constants.Comments.BooleanParameterEndingPhraseTemplate.FormatWith('|').Split('|');
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

#if NCRUNCH
        // do not define a static ctor to speed up tests
#else
        static MiKo_2023_CodeFixProvider() => LoadData(); // ensure that we have the object available
#endif

        public override string FixableDiagnosticId => "MiKo_2023";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

//// ncrunch: no coverage end
//// ncrunch: rdi default

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index, Diagnostic issue)
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
            var count = contents.Count;

            switch (count)
            {
                case 0:
                    return FixEmptyComment(comment.WithContent(XmlText(string.Empty)));

                case 1 when contents[0] is XmlTextSyntax t:
                {
                    var text = t.GetTextWithoutTrivia();

                    if (text.IsEmpty)
                    {
                        return FixEmptyComment(comment);
                    }

                    // determine whether we have a comment like:
                    //    true: some condition
                    //    false: some other condition'
                    var replacement = text.Contains(':') ? ReplacementTo : Replacement;

                    var data = FindMatchingReplacementMapKeysInUpperCase(text);
                    var keysInUpperCase = data.KeysInUpperCase;
                    var length = keysInUpperCase.Length;

                    for (var index = 0; index < length; index++)
                    {
                        var key = keysInUpperCase[index];

                        if (text.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                        {
                            var subText = text.Slice(key.Length)
                                              .TrimStart(Constants.TrailingSentenceMarkers)
                                              .TrimEnd(Constants.TrailingSentenceMarkers);

                            return FixTextOnlyComment(comment, t, subText, replacement, data);
                        }
                    }

                    // seems we could not fix the part
                    var otherPhraseStart = text.IndexOfAny(ElseConditionals, StringComparison.OrdinalIgnoreCase);

                    if (otherPhraseStart > -1)
                    {
                        var subText = text.Slice(0, otherPhraseStart)
                                          .TrimStart(Constants.TrailingSentenceMarkers)
                                          .TrimEnd(Constants.TrailingSentenceMarkers);

                        return FixTextOnlyComment(comment, t, subText, replacement, FindMatchingReplacementMapKeysInUpperCase(ReadOnlySpan<char>.Empty));
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

            return bothFixed;
        }

        private static XmlElementSyntax FixTextOnlyComment(XmlElementSyntax comment, XmlTextSyntax originalText, ReadOnlySpan<char> subText, string replacement, ConcreteMapInfo info)
        {
            subText = ModifyOrNotPart(subText);

            var length = Conditionals.Length;

            for (var index = 0; index < length; index++)
            {
                var conditional = Conditionals[index];

                if (subText.StartsWith(conditional, StringComparison.OrdinalIgnoreCase))
                {
                    subText = subText.Slice(conditional.Length).TrimStart();

                    replacement = ReplacementTo;

                    break;
                }
            }

            var commentContinuation = new StringBuilder();

            // be aware of a gerund verb
            if (replacement == ReplacementTo || Verbalizer.IsGerundVerb(subText.FirstWord()))
            {
                commentContinuation.Append(ReplacementTo);

                var continuation = MakeFirstWordInfiniteVerb(subText, FirstWordHandling.MakeLowerCase);

                commentContinuation.Append(continuation);
            }
            else
            {
                commentContinuation.Append(replacement);

                // do not try to make the first word a verb as it might not be one
                var continuation = subText.TrimStart().ToLowerCaseAt(0);

                commentContinuation.Append(continuation);
            }

            commentContinuation.ReplaceAllWithCheck(info.Map);

            var prepared = comment.ReplaceNode(originalText, XmlText(string.Empty));

            return FixComment(prepared, info.Keys, info.Map, commentContinuation.ToString());
        }

        private static XmlElementSyntax FixComment(XmlElementSyntax prepared, ReadOnlySpan<string> replacementMapKeys, ReadOnlySpan<Pair> replacementMap, string commentContinue = null)
        {
            var startFixed = CommentStartingWith(prepared, StartPhraseParts0, SeeLangword_True(), commentContinue ?? StartPhraseParts1);
            var bothFixed = CommentEndingWith(startFixed, EndPhraseParts0, SeeLangword_False(), EndPhraseParts1);

            var fixedComment = Comment(bothFixed, replacementMapKeys, replacementMap);

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

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            // Fix <see langword>, <b> or <c> by replacing them with nothing
            var result = RemoveBooleansTags(comment);

            // convert first word in infinite verb (if applicable)
            return MakeFirstWordInfiniteVerb(result);
        }

//// ncrunch: rdi off
//// ncrunch: no coverage start

        private static ReadOnlySpan<char> ModifyOrNotPart(ReadOnlySpan<char> text) => text.WithoutSuffix(OrNotPhrase);

        private static ConcreteMapInfo FindMatchingReplacementMapKeysInUpperCase(ReadOnlySpan<char> text)
        {
            // now get all data
            var mappedDataValue = MappedData.Value;

            if (text.StartsWith(StartWithArticleA, StringComparison.Ordinal))
            {
                return new ConcreteMapInfo(mappedDataValue.ReplacementMapForA, mappedDataValue.ReplacementMapKeysForA, mappedDataValue.ReplacementMapKeysInUpperCaseForA);
            }

            if (text.StartsWith(StartWithArticleAn, StringComparison.Ordinal))
            {
                return new ConcreteMapInfo(mappedDataValue.ReplacementMapForAn, mappedDataValue.ReplacementMapKeysForAn, mappedDataValue.ReplacementMapKeysInUpperCaseForAn);
            }

            if (text.StartsWith(StartWithArticleThe, StringComparison.Ordinal))
            {
                return new ConcreteMapInfo(mappedDataValue.ReplacementMapForThe, mappedDataValue.ReplacementMapKeysForThe, mappedDataValue.ReplacementMapKeysInUpperCaseForThe);
            }

            if (text.StartsWith(StartWithParenthesis, StringComparison.OrdinalIgnoreCase))
            {
                return new ConcreteMapInfo(mappedDataValue.ReplacementMapForParenthesis, mappedDataValue.ReplacementMapKeysForParenthesis, mappedDataValue.ReplacementMapKeysInUpperCaseForParenthesis);
            }

            if (text.StartsWith(StartWithArticleLowerCaseA, StringComparison.Ordinal))
            {
                return new ConcreteMapInfo(mappedDataValue.ReplacementMapForLowerCaseA, mappedDataValue.ReplacementMapKeysForLowerCaseA, mappedDataValue.ReplacementMapKeysInUpperCaseForA);
            }

            if (text.StartsWith(StartWithArticleLowerCaseAn, StringComparison.Ordinal))
            {
                return new ConcreteMapInfo(mappedDataValue.ReplacementMapForLowerCaseAn, mappedDataValue.ReplacementMapKeysForLowerCaseAn, mappedDataValue.ReplacementMapKeysInUpperCaseForAn);
            }

            if (text.StartsWith(StartWithArticleLowerCaseThe, StringComparison.Ordinal))
            {
                return new ConcreteMapInfo(mappedDataValue.ReplacementMapForLowerCaseThe, mappedDataValue.ReplacementMapKeysForLowerCaseThe, mappedDataValue.ReplacementMapKeysInUpperCaseForThe);
            }

            return new ConcreteMapInfo(mappedDataValue.ReplacementMapForOthers, mappedDataValue.ReplacementMapKeysForOthers, mappedDataValue.ReplacementMapKeysInUpperCaseForOthers);
        }

        private ref struct ConcreteMapInfo
        {
            public ConcreteMapInfo(ReadOnlySpan<Pair> map, ReadOnlySpan<string> keys, ReadOnlySpan<string> keysInUpperCase)
            {
                Map = map;
                Keys = keys;
                KeysInUpperCase = keysInUpperCase;
            }

            public ReadOnlySpan<Pair> Map { get; }

            public ReadOnlySpan<string> Keys { get; }

            public ReadOnlySpan<string> KeysInUpperCase { get; }
        }

        private sealed class MapData
        {
            public MapData()
            {
                var replacementMapCommon = new[]
                                               {
                                                   new Pair("'true'", string.Empty),
                                                   new Pair("'True'", string.Empty),
                                                   new Pair("'TRUE'", string.Empty),
                                                   new Pair("\"true\"", string.Empty),
                                                   new Pair("\"True\"", string.Empty),
                                                   new Pair("\"TRUE\"", string.Empty),
                                                   new Pair("true", string.Empty),
                                                   new Pair("True", string.Empty),
                                                   new Pair("TRUE", string.Empty),
                                                   new Pair("'false'", string.Empty),
                                                   new Pair("'False'", string.Empty),
                                                   new Pair("'FALSE'", string.Empty),
                                                   new Pair("\"false\"", string.Empty),
                                                   new Pair("\"False\"", string.Empty),
                                                   new Pair("\"FALSE\"", string.Empty),
                                                   new Pair("false", string.Empty),
                                                   new Pair("False", string.Empty),
                                                   new Pair("FALSE", string.Empty),
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
                replacementMapKeysForOthersHashSet.AddRange(replacementMapKeys.Except(replacementMapKeysForAHashSet)
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

                ReplacementMapKeysInUpperCaseForA = ToUpper(replacementMapKeysForA);
                ReplacementMapKeysInUpperCaseForAn = ToUpper(replacementMapKeysForAn);
                ReplacementMapKeysInUpperCaseForThe = ToUpper(replacementMapKeysForThe);
                ReplacementMapKeysInUpperCaseForParenthesis = ToUpper(replacementMapKeysForParenthesis);
                ReplacementMapKeysInUpperCaseForOthers = ToUpper(replacementMapKeysForOthersHashSet);

                // now set keys here at the end as we want these keys sorted based on string contents (and only contain the smallest sub-sequences)
                ReplacementMapKeysForA = GetTermsForQuickLookup(ReplacementMapKeysInUpperCaseForA);
                ReplacementMapKeysForAn = GetTermsForQuickLookup(ReplacementMapKeysInUpperCaseForAn);
                ReplacementMapKeysForThe = GetTermsForQuickLookup(ReplacementMapKeysInUpperCaseForThe);
                ReplacementMapKeysForLowerCaseA = ReplacementMapKeysForA;
                ReplacementMapKeysForLowerCaseAn = ReplacementMapKeysForAn;
                ReplacementMapKeysForLowerCaseThe = ReplacementMapKeysForThe;
                ReplacementMapKeysForParenthesis = GetTermsForQuickLookup(ReplacementMapKeysInUpperCaseForParenthesis);
                ReplacementMapKeysForOthers = GetTermsForQuickLookup(ReplacementMapKeysInUpperCaseForOthers);

                string[] ToKeyArray(IEnumerable<string> keys, string text) => keys.Where(_ => _.StartsWith(text, StringComparison.Ordinal)).ToArray();

                Pair[] ToMapArray(ReadOnlySpan<Pair> map, HashSet<string> keys, Pair[] others)
                {
                    var results = new Pair[keys.Count + others.Length];
                    var resultIndex = 0;

                    var count = map.Length;

                    for (var index = 0; index < count; index++)
                    {
                        var key = map[index];

                        if (keys.Contains(key.Key))
                        {
                            results[resultIndex++] = key;
                        }
                    }

                    others.CopyTo(results, resultIndex);

                    Array.Resize(ref results, resultIndex + others.Length);

                    return results;
                }

                string[] ToUpper(IEnumerable<string> strings) => strings.ToHashSet(_ => _.ToUpperInvariant()).ToArray();
            }

            public Pair[] ReplacementMapForA { get; }

            public Pair[] ReplacementMapForAn { get; }

            public Pair[] ReplacementMapForThe { get; }

            public Pair[] ReplacementMapForParenthesis { get; }

            public Pair[] ReplacementMapForOthers { get; }

            public Pair[] ReplacementMapForLowerCaseA { get; }

            public Pair[] ReplacementMapForLowerCaseAn { get; }

            public Pair[] ReplacementMapForLowerCaseThe { get; }

            public string[] ReplacementMapKeysForA { get; }

            public string[] ReplacementMapKeysForAn { get; }

            public string[] ReplacementMapKeysForThe { get; }

            public string[] ReplacementMapKeysForParenthesis { get; }

            public string[] ReplacementMapKeysForOthers { get; }

            public string[] ReplacementMapKeysForLowerCaseA { get; }

            public string[] ReplacementMapKeysForLowerCaseAn { get; }

            public string[] ReplacementMapKeysForLowerCaseThe { get; }

            public string[] ReplacementMapKeysInUpperCaseForA { get; }

            public string[] ReplacementMapKeysInUpperCaseForAn { get; }

            public string[] ReplacementMapKeysInUpperCaseForThe { get; }

            public string[] ReplacementMapKeysInUpperCaseForParenthesis { get; }

            public string[] ReplacementMapKeysInUpperCaseForOthers { get; }

            private static Pair[] CreateReplacementMap()
            {
                var startTerms = CreateStartTerms();

                var textsCount = startTerms.Count;

                var replacements = new Pair[textsCount];
                var index = 0;

                foreach (var text in startTerms.OrderBy(_ => _, ArticleStartComparer).ThenByDescending(_ => _.Length).ThenBy(_ => _))
                {
                    replacements[index++] = new Pair(text, Replacement);
                }

                return replacements;
            }

            // ReSharper disable once ReturnTypeCanBeEnumerable.Local Violates CA1859
            private static HashSet<string> CreateStartTerms()
            {
                var startTerms = new[] { "A", "An", "The", string.Empty };
                var optionals = new[] { "Optional", "(optional)", "(Optional)", "optional", string.Empty };
                var booleans = new[] { "bool", "Bool", "boolean", "Boolean", string.Empty };
                var parameters = new[] { "flag", "Flag", "value", "Value", "parameter", "Parameter", "paramter", "Paramter", string.Empty };

                var starts = new List<string>((startTerms.Length * optionals.Length * booleans.Length * parameters.Length) + 1) { string.Empty };

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var startTerm in startTerms)
                {
                    // we have lots of loops, so cache data to avoid unnecessary calculations
                    var s = startTerm + " ";

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var optional in optionals)
                    {
                        // we have lots of loops, so cache data to avoid unnecessary calculations
                        var optionalStart = s + optional + " ";

                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (var boolean in booleans)
                        {
                            // we have lots of loops, so cache data to avoid unnecessary calculations
                            var optionalBooleanStart = new StringBuilder(optionalStart).Append(boolean).Append(' ').ReplaceWithCheck("   ", " ").ReplaceWithCheck("  ", " ").TrimStart();

                            // ReSharper disable once LoopCanBeConvertedToQuery
                            foreach (var parameter in parameters)
                            {
                                var fixedStart = new StringBuilder(optionalBooleanStart).Append(parameter).TrimEnd();

                                if (fixedStart.IsNullOrWhiteSpace() is false)
                                {
                                    starts.Add(fixedStart);
                                }
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
                    var end = " " + condition + " ";

                    // for performance reasons we use for loops here
                    for (var verbIndex = 0; verbIndex < verbsLength; verbIndex++)
                    {
                        var verb = verbs[verbIndex];
                        var middle = " " + verb + end;

                        // for performance reasons we use for loops here
                        for (var startIndex = 0; startIndex < startsCount; startIndex++)
                        {
                            var start = starts[startIndex];
                            var text = new StringBuilder(start).Append(middle).TrimStart();

                            results.Add(text.ToUpperCaseAt(0));
                            results.Add(text.ToLowerCaseAt(0));
                        }
                    }

                    // for performance reasons we use for loops here
                    for (var index = 0; index < startingVerbsLength; index++)
                    {
                        var startingVerb = startingVerbs[index];
                        var text = startingVerb + end;

                        results.Add(text.ToUpperCaseAt(0));
                        results.Add(text.ToLowerCaseAt(0));
                    }
                }

                return results;
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
                    return GetOrder(x) - GetOrder(y);
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

            private int GetOrder(string text)
            {
                var length = m_specialOrder.Length;

                for (var i = 0; i < length; i++)
                {
                    var order = m_specialOrder[i];

                    if (text.StartsWith(order, StringComparison.Ordinal))
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