using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2233_CodeFixProvider)), Shared]
    public sealed class MiKo_2233_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2233";

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case XmlEmptyElementSyntax element: return GetUpdatedSyntax(element);
                case XmlElementStartTagSyntax start: return GetUpdatedSyntax(start);
                case XmlElementEndTagSyntax end: return GetUpdatedSyntax(end);
                default:
                    return null;
            }
        }

        private static XmlElementStartTagSyntax GetUpdatedSyntax(XmlElementStartTagSyntax tag) => tag.WithLessThanToken(tag.LessThanToken.WithoutTrivia())
                                                                                                     .WithName(tag.Name.WithoutTrivia())
                                                                                                     .WithAttributes(tag.Attributes.Select(GetUpdatedSyntax).ToSyntaxList())
                                                                                                     .WithGreaterThanToken(tag.GreaterThanToken.WithoutTrivia());

        private static XmlElementEndTagSyntax GetUpdatedSyntax(XmlElementEndTagSyntax tag) => tag.WithLessThanSlashToken(tag.LessThanSlashToken.WithoutTrivia())
                                                                                                 .WithName(tag.Name.WithoutTrivia())
                                                                                                 .WithGreaterThanToken(tag.GreaterThanToken.WithoutTrivia());

        private static XmlEmptyElementSyntax GetUpdatedSyntax(XmlEmptyElementSyntax element) => element.WithLessThanToken(element.LessThanToken.WithoutTrivia())
                                                                                                       .WithName(element.Name.WithoutTrivia())
                                                                                                       .WithAttributes(element.Attributes.Select(GetUpdatedSyntax).ToSyntaxList())
                                                                                                       .WithSlashGreaterThanToken(element.SlashGreaterThanToken.WithoutTrivia());

        private static XmlAttributeSyntax GetUpdatedSyntax(XmlAttributeSyntax attribute) => attribute.Without(attribute.DescendantTrivia()).WithLeadingSpace();
    }
}