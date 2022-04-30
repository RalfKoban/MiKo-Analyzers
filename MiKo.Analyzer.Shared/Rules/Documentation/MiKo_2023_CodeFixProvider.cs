using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2023_CodeFixProvider)), Shared]
    public sealed class MiKo_2023_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        private static readonly KeyValuePair<string, string>[] ReplacementMap =
            {
                new KeyValuePair<string, string>("true", string.Empty),
                new KeyValuePair<string, string>("false", string.Empty),
                new KeyValuePair<string, string>(" to determine whether ", " to "),
                new KeyValuePair<string, string>(" to determines whether ", " to "),
                new KeyValuePair<string, string>(" to indicate whether ", " to "),
                new KeyValuePair<string, string>(" to indicates whether ", " to "),
                new KeyValuePair<string, string>(" to if set to ", " to "),
                new KeyValuePair<string, string>(" to if ", " to "),
                new KeyValuePair<string, string>(" to  if ", " to "),
                new KeyValuePair<string, string>(" to to ", " to "),
                new KeyValuePair<string, string>(" to  to ", " to "),
                new KeyValuePair<string, string>(" otherwise; otherwise, ", "otherwise, "),
                new KeyValuePair<string, string>("; otherwise ", string.Empty),
                new KeyValuePair<string, string>(". Otherwise ", string.Empty),
                new KeyValuePair<string, string>(". ", "; "),
            };

        private static readonly IEnumerable<string> ReplacementMapKeys = ReplacementMap.Select(_ => _.Key).ToList();

        private static readonly string[] StartPhraseParts = Constants.Comments.BooleanParameterStartingPhraseTemplate.FormatWith('|').Split('|');
        private static readonly string[] EndPhraseParts = Constants.Comments.BooleanParameterEndingPhraseTemplate.FormatWith('|').Split('|');

        public override string FixableDiagnosticId => MiKo_2023_BooleanParamDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2023_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax Comment(CodeFixContext context, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic) => comment; // TODO RKN: fix

        protected override XmlElementSyntax Comment(CodeFixContext context, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            var preparedComment = PrepareComment(comment);

            var startFixed = CommentStartingWith(preparedComment, StartPhraseParts[0], SeeLangword_True(), StartPhraseParts[1]);
            var bothFixed = CommentEndingWith(startFixed, EndPhraseParts[0], SeeLangword_False(), EndPhraseParts[1]);

            var fixedComment = Comment(bothFixed, ReplacementMapKeys, ReplacementMap);

            return fixedComment;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            // Fix <see langword>, <b> or <c> by replacing them with nothing
            var result = RemoveBooleansTags(comment);

            // convert first word in infinite verb (if applicable)
            return MakeFirstWordInfiniteVerb(result);
        }
    }
}