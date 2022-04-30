using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2006_CodeFixProvider)), Shared]
    public sealed class MiKo_2006_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly string[] SummaryText = Constants.Comments.RoutedEventFieldSummaryPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string[] ValueText = Constants.Comments.RoutedEventFieldValuePhraseTemplate.FormatWith("|").Split('|');

        public override string FixableDiagnosticId => MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2006_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var fieldDeclaration = syntax.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            if (fieldDeclaration is null)
            {
                return syntax;
            }

            var fieldName = fieldDeclaration.Declaration.Variables.First().Identifier.ValueText;
            var name = fieldName.WithoutSuffix(Constants.RoutedEventFieldSuffix);

            var type = SyntaxFactory.ParseName(name);

            var readOnlyMarker = string.Empty;
            if (fieldDeclaration.Modifiers.Any(_ => _.IsKind(SyntaxKind.ReadOnlyKeyword)))
            {
                readOnlyMarker = " " + Constants.Comments.FieldIsReadOnly;
            }

            var summary = Comment(XmlElement(Constants.XmlTag.Summary), SummaryText[0], SeeCref(type), SummaryText[1] + readOnlyMarker);
            var field = Comment(XmlElement(Constants.XmlTag.Value), ValueText[0], SeeCref(type), ValueText[1]);

            return syntax.WithoutTrivia()
                         .WithContent(SyntaxFactory.List<XmlNodeSyntax>(new[]
                                                                            {
                                                                                summary.WithTrailingXmlComment(),
                                                                                field.WithEndOfLine(),
                                                                            }))
                         .WithLeadingXmlCommentExterior();
        }
    }
}