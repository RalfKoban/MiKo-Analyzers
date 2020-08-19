using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2010_CodeFixProvider)), Shared]
    public sealed class MiKo_2010_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2010_SealedClassSummaryAnalyzer.Id;

        protected override string Title => "Append sealed text";

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;

            const string SealedText = Constants.Comments.SealedClassPhrase;

            return SyntaxFactory.XmlElement(
                                        comment.StartTag,
                                        comment.WithoutText(SealedText).Add(SyntaxFactory.XmlText(SealedText)),
                                        comment.EndTag.WithLeadingXmlComment()); // place on new line
        }
    }
}