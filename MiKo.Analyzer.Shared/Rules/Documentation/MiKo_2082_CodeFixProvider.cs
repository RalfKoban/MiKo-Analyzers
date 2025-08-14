﻿using System;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2082_CodeFixProvider)), Shared]
    public sealed class MiKo_2082_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] ReplacementMapKeys = CreateReplacementMapKeys().ToArray();

        private static readonly Pair[] ReplacementMap = ReplacementMapKeys.ToArray(_ => new Pair(_));

        private static readonly string[] TypesSuffixes = { "Types", "Type", "Enum" };

        private static readonly string[] KindEndings = { " kinds", " kind" };

        private static readonly string[] WordsThatPreventArticle = { "On", "Off", "None", "Undefined" };

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2082";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            var contents = comment.Content;

            if (contents.Count is 1 && contents[0] is XmlTextSyntax txt)
            {
                var text = txt.GetTextTrimmed();

                if (text.StartsWith("Enum", StringComparison.Ordinal))
                {
                    var enumMember = txt.FirstAncestor<EnumMemberDeclarationSyntax>();

                    var enumMemberName = enumMember?.GetName();
                    var unsuffixedSpan = enumMemberName.AsSpan().WithoutSuffix("Enum");
                    var unsuffixed = unsuffixedSpan.ToString();

                    var start = "Enum " + enumMemberName;
                    var startWithFor = start + " for ";
                    var startWithEnumFor = start + "Enum for ";

                    var startPhrases = new[]
                                           {
                                               startWithFor + unsuffixed,
                                               startWithEnumFor + enumMemberName,
                                           };

                    if (text.StartsWithAny(startPhrases, StringComparison.OrdinalIgnoreCase))
                    {
                        var enumType = enumMember.FirstAncestor<EnumDeclarationSyntax>().GetName();

                        string article;

                        var isPlural = Pluralizer.IsPlural(unsuffixedSpan) && Pluralizer.IsSingularAndPlural(unsuffixedSpan) is false;

                        if (isPlural
                         || unsuffixed.EqualsAny(WordsThatPreventArticle, StringComparison.OrdinalIgnoreCase)
                         || Verbalizer.IsGerundVerb(unsuffixedSpan)
                         || Verbalizer.IsAdjectiveOrAdverb(unsuffixedSpan)
                         || Verbalizer.IsPastTense(unsuffixedSpan))
                        {
                            // prevent articles for gerund and past tense words and make them lower case
                            article = string.Empty;
                        }
                        else
                        {
                            article = ArticleProvider.GetArticleFor(unsuffixedSpan, FirstWordAdjustment.StartLowerCase);
                        }

                        unsuffixed = unsuffixed.ToLowerCaseAt(0);

                        var firstWordHandling = FirstWordAdjustment.StartLowerCase;

                        if (isPlural)
                        {
                            firstWordHandling |= FirstWordAdjustment.MakePlural;
                        }

                        var replacement = enumType.AsCachedBuilder()
                                                  .Without(TypesSuffixes)
                                                  .SeparateWords(' ', firstWordHandling)
                                                  .Without(KindEndings)
                                                  .Insert(0, "The ")
                                                  .Append(isPlural ? " are " : " is ")
                                                  .Append(article)
                                                  .Append(unsuffixed)
                                                  .Append('.')
                                                  .ToStringAndRelease();

                        return Comment(comment, replacement);
                    }
                }
            }

            return Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordAdjustment.StartUpperCase | FirstWordAdjustment.KeepSingleLeadingSpace);
        }

//// ncrunch: rdi off

        private static IEnumerable<string> CreateReplacementMapKeys()
        {
            var continuations = new[] { "that", "whether", "for" };

            foreach (var start in Constants.Comments.EnumMemberWrongStartingWords)
            {
                foreach (var continuation in continuations)
                {
                    yield return string.Concat(start, ", ", continuation, " ");
                    yield return string.Concat(start, " ", continuation, " ");
                }

                yield return start + " ";
            }
        }

//// ncrunch: rdi default
    }
}