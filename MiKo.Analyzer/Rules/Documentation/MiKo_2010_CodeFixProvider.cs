using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2010_CodeFixProvider)), Shared]
    public sealed class MiKo_2010_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2010_SealedClassSummaryAnalyzer.Id;

        protected override string Title => "Append sealed text";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(Constants.XmlTag.Summary, syntaxNodes).First();

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;

            const string SealedText = Constants.Comments.SealedClassPhrase;

            var contents = comment.WithoutText(SealedText)
                                  .Add(SyntaxFactory.XmlText(SealedText).WithTrailingTrivia(XmlCommentStart)); // place on new line

            return SyntaxFactory.XmlElement(comment.StartTag, contents, comment.EndTag);
        }
    }
}