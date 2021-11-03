using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2001_CodeFixProvider)), Shared]
    public sealed class MiKo_2001_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string SpecialTerm = "Occurs that ";
        private static readonly string[] SpecialTerms = { SpecialTerm };

        private static readonly KeyValuePair<string, string>[] SpecialTermReplacementMap = { new KeyValuePair<string, string>(SpecialTerm, "Occurs when ") };

        private static readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "Event fired ", string.Empty },
                                                                                    { "Event raised ", string.Empty },
                                                                                    { "Event is fired ", string.Empty },
                                                                                    { "Event is raised ", string.Empty },
                                                                                    { "Event occurs ", string.Empty },
                                                                                    { "Event shall be fired ", string.Empty },
                                                                                    { "Event shall be raised ", string.Empty },
                                                                                    { "Event that is fired ", string.Empty },
                                                                                    { "Event that is raised ", string.Empty },
                                                                                    { "Event that shall be fired ", string.Empty },
                                                                                    { "Event that shall be raised ", string.Empty },
                                                                                    { "Event to fire ", string.Empty },
                                                                                    { "Event to raise ", string.Empty },
                                                                                    { "Event which is fired ", string.Empty },
                                                                                    { "Event which is raised ", string.Empty },
                                                                                    { "Fired ", string.Empty },
                                                                                    { "Indicates ", string.Empty },
                                                                                    { "Is fired ", string.Empty },
                                                                                    { "Is raised ", string.Empty },
                                                                                    { "Raised ", string.Empty },
                                                                                    { "Shall be fired ", string.Empty },
                                                                                    { "Shall be raised ", string.Empty },
                                                                                    { "The event to fire ", string.Empty },
                                                                                    { "The event to raise ", string.Empty },
                                                                                    { "This event is fired ", string.Empty },
                                                                                    { "This event is raised ", string.Empty },
                                                                                    { "This event occurs ", string.Empty },
                                                                                    { "This event shall be fired ", string.Empty },
                                                                                    { "This event shall be raised ", string.Empty },
                                                                                };

        public override string FixableDiagnosticId => MiKo_2001_EventSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2001_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var preparedComment = PrepareComment((XmlElementSyntax)syntax);

            var fixedComment = CommentStartingWith(preparedComment, MiKo_2001_EventSummaryAnalyzer.Phrase);

            var text = fixedComment.Content[0].WithoutXmlCommentExterior();
            if (text.StartsWith(SpecialTerm, StringComparison.Ordinal))
            {
                return Comment(fixedComment, SpecialTerms, SpecialTermReplacementMap);
            }

            return fixedComment;
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMap.Keys, ReplacementMap);
    }
}