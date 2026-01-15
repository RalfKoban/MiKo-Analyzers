using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2074_CodeFixProvider)), Shared]
    public sealed class MiKo_2074_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap =
                                                        {
                                                            new Pair("to search to seek", "to seek"),
                                                            new Pair("to search for to seek", "to seek"),
                                                            new Pair("to look up to seek", "to seek"),
                                                            new Pair("to look-up to seek", "to seek"),
                                                            new Pair("to check to seek", "to seek"),
                                                            new Pair("to check for to seek", "to seek"),
                                                            new Pair("to check if contained to seek", "to seek"),
                                                        };

        private static readonly string[] ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap);

        public override string FixableDiagnosticId => "MiKo_2074";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, in int index, Diagnostic issue)
        {
            var phrase = GetPhraseProposal(issue);

            if (comment.Content.Count is 0)
            {
                // we do not have a comment
                return comment.WithContent(XmlText("The item" + phrase));
            }

            var updatedComment = CommentEndingWith(comment, phrase);

            return Comment(updatedComment, ReplacementMapKeys, ReplacementMap);
        }
    }
}