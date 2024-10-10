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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2082_CodeFixProvider)), Shared]
    public sealed class MiKo_2082_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] ReplacementMapKeys = CreateReplacementMapKeys().ToArray();

        private static readonly Pair[] ReplacementMap = ReplacementMapKeys.ToArray(_ => new Pair(_, string.Empty));

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2082";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;

            var contents = comment.Content;

            if (contents.Count == 1 && contents[0] is XmlTextSyntax txt)
            {
                var text = txt.GetTextWithoutTrivia();

                if (text.StartsWith("Enum"))
                {
                    var enumMember = txt.FirstAncestor<EnumMemberDeclarationSyntax>();

                    var enumMemberName = enumMember?.GetName();
                    var unsuffixed = enumMemberName.AsSpan().WithoutSuffix("Enum").ToString();

                    var start = "Enum " + enumMemberName;

                    var startPhrases = new[]
                                           {
                                               start + " for " + unsuffixed,
                                               start + "Enum for " + enumMemberName,
                                           };

                    if (text.StartsWithAny(startPhrases, StringComparison.OrdinalIgnoreCase))
                    {
                        var enumType = enumMember.FirstAncestor<EnumDeclarationSyntax>().GetName().Without("Type").Without("Enum");
                        var separated = WordSeparator.Separate(enumType, ' ', FirstWordHandling.MakeLowerCase);
                        var article = ArticleProvider.GetArticleFor(unsuffixed, FirstWordHandling.MakeLowerCase);

                        var replacement = new StringBuilder("The ").Append(separated)
                                                                   .Append(" is ")
                                                                   .Append(article)
                                                                   .Append(unsuffixed)
                                                                   .Append('.')
                                                                   .ToString();

                        return Comment(comment, replacement);
                    }
                }
            }

            return Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordHandling.MakeUpperCase | FirstWordHandling.KeepLeadingSpace);
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