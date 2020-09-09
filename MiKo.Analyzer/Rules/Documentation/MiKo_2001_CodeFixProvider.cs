using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2001_CodeFixProvider)), Shared]
    public sealed class MiKo_2001_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string Phrase = MiKo_2001_EventSummaryAnalyzer.Phrase;

        private static readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "This event is fired ", string.Empty },
                                                                                    { "This event is raised ", string.Empty },
                                                                                    { "This event occurs ", string.Empty },
                                                                                    { "Event is fired ", string.Empty },
                                                                                    { "Event is raised ", string.Empty },
                                                                                    { "Event occurs ", string.Empty },
                                                                                    { "Is fired ", string.Empty },
                                                                                    { "Is raised ", string.Empty },
                                                                                    { "Fired ", string.Empty },
                                                                                    { "Raised ", string.Empty },
                                                                                    { "Indicates ", string.Empty },
                                                                                };

        public override string FixableDiagnosticId => MiKo_2001_EventSummaryAnalyzer.Id;

        protected override string Title => "Start comment with '" + Phrase + "'";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var preparedComment = PrepareComment((XmlElementSyntax)syntax);
            var fixedComment = CommentStartingWith(preparedComment, Phrase);

            var text = fixedComment.Content[0].WithoutXmlCommentExterior();

            if (text.StartsWith("Occurs that ", StringComparison.Ordinal))
            {
                return Comment(
                               fixedComment,
                               new[] { "Occurs that " },
                               new[] { new KeyValuePair<string, string>("Occurs that ", "Occurs when ") });
            }

            return fixedComment;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMap.Select(_ => _.Key).ToList(), ReplacementMap);
    }
}