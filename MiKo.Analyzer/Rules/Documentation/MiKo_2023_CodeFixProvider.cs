﻿using System.Collections.Generic;
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
        private static readonly HashSet<string> Attributes = new HashSet<string>
                                                                 {
                                                                     Constants.XmlTag.Attribute.Langword,
                                                                     Constants.XmlTag.Attribute.Langref,
                                                                 };

        private static readonly HashSet<string> Booleans = new HashSet<string>
                                                                 {
                                                                     "true",
                                                                     "false",
                                                                 };

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

        private static readonly string[] StartPhraseParts = string.Format(Constants.Comments.BooleanParameterStartingPhraseTemplate, '|').Split('|');
        private static readonly string[] EndPhraseParts = string.Format(Constants.Comments.BooleanParameterEndingPhraseTemplate, '|').Split('|');

        public override string FixableDiagnosticId => MiKo_2023_BooleanParamDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2023_CodeFixTitle;

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            var preparedComment = PrepareComment(comment);

            var startFixed = CommentStartingWith(preparedComment, StartPhraseParts[0], SeeLangword_True(), StartPhraseParts[1]);
            var bothFixed = CommentEndingWith(startFixed, EndPhraseParts[0], SeeLangword_False(), EndPhraseParts[1]);

            var fixedComment = Comment(bothFixed, ReplacementMapKeys, ReplacementMap);

            return fixedComment;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            // Fix <see langword> or <c> by replacing them with nothing
            var nodes = Enumerable.Empty<SyntaxNode>()
                                  .Concat(comment.Content.OfType<XmlEmptyElementSyntax>()
                                                 .Where(_ => _.GetName() == Constants.XmlTag.See && Attributes.Contains(_.Attributes.FirstOrDefault().GetName())))
                                  .Concat(comment.Content.OfType<XmlElementSyntax>()
                                                 .Where(_ => _.GetName() == Constants.XmlTag.C && Booleans.Contains(_.Content.ToString())))
                                  .ToList();

            return comment.RemoveNodes(nodes, SyntaxRemoveOptions.KeepNoTrivia);
        }
    }
}