using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2051_CodeFixProvider)), Shared]
    public sealed class MiKo_2051_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        // TODO RKN: see Constants.Comments.ExceptionForbiddenStartingPhrase
        private static readonly string[] Phrases =
            {
                "gets thrown when the ",
                "Gets thrown when the ",
                "is thrown when the ",
                "Is thrown when the ",
                "gets thrown when ",
                "Gets thrown when ",
                "thrown when the ",
                "Thrown when the ",
                "throws when the ",
                "Throws when the ",
                "is thrown when ",
                "Is thrown when ",
                "thrown if the ",
                "Thrown if the ",
                "throws if the ",
                "Throws if the ",
                "throws when ",
                "Throws when ",
                "thrown when ",
                "Thrown when ",
                "thrown if ",
                "Thrown if ",
                "throws if ",
                "Throws if ",
                "in case the ",
                "In case the ",
                "In case the ",
                "in case ",
                "In case ",
                "In case ",
                "if the ",
                "If the ",
                "If ",
            };

        public override string FixableDiagnosticId => MiKo_2051_ExceptionTagDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2051_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var updatedSyntax = syntax.ReplaceNodes(
                                                    syntax.GetExceptionXmls(),
                                                    (original, rewritten)
                                                        =>
                                                        {
                                                            if (original.IsExceptionCommentFor<ArgumentNullException>())
                                                            {
                                                                return GetFixedExceptionCommentForArgumentNullException(original);
                                                            }

                                                            if (original.IsExceptionCommentFor<ObjectDisposedException>())
                                                            {
                                                                return original.WithContent(XmlText(Constants.Comments.ObjectDisposedExceptionPhrase).WithLeadingXmlComment().WithTrailingXmlComment());
                                                            }

                                                            var replaced = original;

                                                            if (original.IsExceptionCommentFor<ArgumentOutOfRangeException>())
                                                            {
                                                                replaced = GetFixedExceptionCommentForArgumentOutOfRangeException(original);
                                                            }

                                                            if (replaced.Content.First() is XmlTextSyntax text)
                                                            {
                                                                replaced = ReplaceText(replaced, text, Phrases, string.Empty);
                                                            }

                                                            return replaced;
                                                        });

            return updatedSyntax;
        }
    }
}