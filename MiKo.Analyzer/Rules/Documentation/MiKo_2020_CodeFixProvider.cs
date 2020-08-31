using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2020_CodeFixProvider)), Shared]
    public sealed class MiKo_2020_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2020_InheritdocSummaryAnalyzer.Id;

        protected override string Title => "Use <inheritdoc/>";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var inheritdoc = SyntaxFactory.XmlEmptyElement(Constants.XmlTag.Inheritdoc);

            return comment.WithoutTrivia()
                          .WithContent(new SyntaxList<XmlNodeSyntax>(inheritdoc.WithEndOfLine()))
                          .WithLeadingTrivia(SyntaxExtensions.XmlCommentExterior);
        }
    }
}