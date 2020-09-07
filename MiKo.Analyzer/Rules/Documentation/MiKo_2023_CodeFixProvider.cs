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

        public override string FixableDiagnosticId => MiKo_2023_BooleanParamDefaultPhraseAnalyzer.Id;

        protected override string Title => "Fix comment start of Boolean parameter";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            var preparedComment = PrepareComment(comment);

            var startPhraseParts = string.Format(Constants.Comments.BooleanParameterStartingPhraseTemplate, '|').Split('|');
            var endPhraseParts = string.Format(Constants.Comments.BooleanParameterEndingPhraseTemplate, '|').Split('|');

            var startFixed = CommentStartingWith(preparedComment, startPhraseParts[0], SeeLangword_True(), startPhraseParts[1]);
            var bothFixed = CommentEndingWith(startFixed, endPhraseParts[0], SeeLangword_False(), endPhraseParts[1]);

            var fixedComment = Comment(bothFixed, ReplacementMap.Select(_ => _.Key).ToList(), ReplacementMap);
            return fixedComment;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment)
        {
            // Fix <see langword> or <c> by replacing them with nothing
            var nodes = Enumerable.Empty<SyntaxNode>()
                                  .Concat(comment.Content.OfType<XmlEmptyElementSyntax>()
                                                 .Where(_ => _.Name.LocalName.ValueText == Constants.XmlTag.See && Attributes.Contains(_.Attributes.FirstOrDefault()?.Name.LocalName.ValueText)))
                                  .Concat(comment.Content.OfType<XmlElementSyntax>()
                                                 .Where(_ => _.StartTag.Name.LocalName.ValueText == Constants.XmlTag.C && Booleans.Contains(_.Content.ToString())))
                                  .ToList();

            return comment.RemoveNodes(nodes, SyntaxRemoveOptions.KeepNoTrivia);
        }
    }
}