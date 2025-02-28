using System;
using System.Composition;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2000_CodeFixProvider)), Shared]
    public sealed class MiKo_2000_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Pair[] XmlEntities =
                                                     {
                                                         new Pair("&", "&amp;"),
                                                         new Pair("<", "&lt;"),
                                                     };

        public override string FixableDiagnosticId => "MiKo_2000";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var token = syntax.FindToken(diagnostic);
            var tokenText = token.Text;

            var text = tokenText.AsCachedBuilder()
                                .ReplaceAllWithCheck(XmlEntities)
                                .ToStringAndRelease();

            return syntax.ReplaceToken(token, token.WithText(text));
        }
    }
}