using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2233_CodeFixProvider)), Shared]
    public sealed class MiKo_2233_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2233";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlEmptyElementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlEmptyElementSyntax element)
            {
                return element.WithLessThanToken(element.LessThanToken.WithoutTrivia())
                              .WithName(element.Name.WithoutTrivia())
                              .WithAttributes(element.Attributes.Select(GetUpdatedSyntax).ToSyntaxList())
                              .WithSlashGreaterThanToken(element.SlashGreaterThanToken.WithoutTrivia());
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }

        private static XmlAttributeSyntax GetUpdatedSyntax(XmlAttributeSyntax attribute) => attribute.WithoutTrivia()
                                                                                                     .WithName(attribute.Name.WithoutTrivia())
                                                                                                     .WithEqualsToken(attribute.EqualsToken.WithoutTrivia())
                                                                                                     .WithStartQuoteToken(attribute.StartQuoteToken.WithoutTrivia())
                                                                                                     .WithEndQuoteToken(attribute.EndQuoteToken.WithoutTrivia())
                                                                                                     .WithLeadingSpace();
    }
}