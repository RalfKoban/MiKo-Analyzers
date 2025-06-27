using System;
using System.Composition;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2000_CodeFixProvider)), Shared]
    public sealed class MiKo_2000_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Pair[] XmlEntities =
                                                     {
                                                         new Pair("&", "&amp;"),
                                                         new Pair("<", "&lt;"),
                                                         new Pair(">", "&gt;"),
                                                     };

        public override string FixableDiagnosticId => "MiKo_2000";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var token = syntax.FindToken(diagnostic);

            if (token.IsKind(SyntaxKind.LessThanToken) && token.Parent is XmlElementStartTagSyntax startTag && startTag.Parent is XmlElementSyntax element)
            {
                // as the XML element now has an end tag that does not match the start tag, we have to fix the complete documentation XML
                return GetUpdatedDocumentationComment(syntax, element);
            }

            var tokenText = token.Text;

            var text = tokenText.AsCachedBuilder()
                                .ReplaceAllWithProbe(XmlEntities)
                                .ToStringAndRelease();

            return syntax.ReplaceToken(token, token.WithText(text));
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedDocumentationComment(DocumentationCommentTriviaSyntax syntax, XmlElementSyntax element)
        {
            // as the XML element now has an end tag that does not match the start tag, we have to fix the complete documentation XML
            var startTag = element.StartTag;

            var location = Location.Create(syntax.SyntaxTree, TextSpan.FromBounds(syntax.SpanStart, startTag.SpanStart));
            var someText = location.GetText();

            var startTagFullString = startTag.ToFullString();
            var contentFullString = element.Content.ToFullString();
            var endTagFullString = element.EndTag.ToFullString();

            var capacity = StringBuilderCache.DefaultCapacity // some buffer as we modify the contents
                         + Constants.Comments.XmlCommentExterior.Length
                         + someText.Length
                         + startTagFullString.Length
                         + contentFullString.Length
                         + endTagFullString.Length;

            var text = StringBuilderCache.Acquire(capacity)
                                         .Append(startTagFullString)
                                         .Append(contentFullString)
                                         .ReplaceAllWithProbe(XmlEntities)
                                         .Insert(0, someText)
                                         .Insert(0, Constants.Comments.XmlCommentExterior)
                                         .AppendLine(endTagFullString)
                                         .ToStringAndRelease();

            var newParent = SyntaxFactory.ParseTypeName("###").WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia(text));

            return newParent.GetDocumentationCommentTriviaSyntax().FirstOrDefault() ?? syntax;
        }
    }
}