using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2074_CodeFixProvider)), Shared]
    public sealed class MiKo_2074_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2074_ContainsParameterDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2074_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax Comment(CodeFixContext context, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic) => comment; // TODO RKN: fix

        protected override XmlElementSyntax Comment(CodeFixContext context, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            if (comment.Content.Count == 0)
            {
                // we do not have a comment
                return comment.WithContent(XmlText("The item" + MiKo_2074_ContainsParameterDefaultPhraseAnalyzer.Phrase));
            }

            return CommentEndingWith(comment, MiKo_2074_ContainsParameterDefaultPhraseAnalyzer.Phrase);
        }
    }
}