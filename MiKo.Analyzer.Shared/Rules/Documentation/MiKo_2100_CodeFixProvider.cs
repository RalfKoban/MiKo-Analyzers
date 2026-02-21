using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2100_CodeFixProvider)), Shared]
    public sealed class MiKo_2100_CodeFixProvider : ExampleDocumentationCodeFixProvider
    {
        private const string Phrase = Constants.Comments.ExampleDefaultPhrase;
        private const string CompletePhrase = Phrase + "its usage.";

        public override string FixableDiagnosticId => "MiKo_2100";

        protected override string Title => Resources.MiKo_2100_CodeFixTitle.FormatWith(Phrase);

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is XmlElementSyntax element)
            {
                var content = element.Content;

                if (content.Count > 0)
                {
                    if (content[0].IsCode())
                    {
                        var xmlText = XmlText(CompletePhrase).WithLeadingXmlComment()
                                                             .WithTrailingNewLine()
                                                             .WithTrailingXmlComment();

                        return element.WithContent(element.Content.Insert(0, xmlText));
                    }

                    if (content[0].IsWhiteSpaceOnlyText() && content.Count > 1 && content[1].IsCode())
                    {
                        var xmlText = XmlText(CompletePhrase).WithTrailingNewLine()
                                                             .WithTrailingXmlComment();

                        return element.WithContent(element.Content.Insert(1, xmlText));
                    }

                    return CommentStartingWith(element, Phrase);
                }
            }

            return syntax;
        }
    }
}