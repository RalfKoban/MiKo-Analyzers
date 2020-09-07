using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2006_CodeFixProvider)), Shared]
    public sealed class MiKo_2006_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzer.Id;

        protected override string Title => "Apply standard comment to RoutedEvent";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var fieldDeclaration = comment.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();
            var fieldName = fieldDeclaration.Declaration.Variables.First().Identifier.ValueText;
            var name = fieldName.WithoutSuffix(Constants.RoutedEventFieldSuffix);

            var type = SyntaxFactory.ParseName(name);

            var readOnlyMarker = string.Empty;
            if (fieldDeclaration.Modifiers.Any(_ => _.IsKind(SyntaxKind.ReadOnlyKeyword)))
            {
                readOnlyMarker = " " + Constants.Comments.FieldIsReadOnly;
            }

            var summaryText = string.Format(Constants.Comments.RoutedEventFieldSummaryPhraseTemplate, "|").Split('|');
            var valueText = string.Format(Constants.Comments.RoutedEventFieldValuePhraseTemplate, "|").Split('|');

            var summary = Comment(SyntaxFactory.XmlElement(Constants.XmlTag.Summary, default), summaryText[0], SeeCref(type), summaryText[1] + readOnlyMarker);
            var field = Comment(SyntaxFactory.XmlElement(Constants.XmlTag.Value, default), valueText[0], SeeCref(type), valueText[1]);

            return comment.WithoutTrivia()
                          .WithContent(SyntaxFactory.List<XmlNodeSyntax>(new[]
                                                                             {
                                                                                 summary.WithTrailingXmlComment(),
                                                                                 field.WithEndOfLine(),
                                                                             }))
                          .WithLeadingTrivia(SyntaxExtensions.XmlCommentExterior);
        }
    }
}