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
                "Thrown if the ",
                "Thrown if ",
                "Thrown when the ",
                "Thrown when ",
                "Throws if the ",
                "Throws if ",
                "Throws when the ",
                "Throws when ",
                "Is thrown when the ",
                "Is thrown when ",
                "Gets thrown when the ",
                "Gets thrown when ",
                "If the ",
                "If ",
                "In case the ",
                "In case ",
                "if the ",
                "in case the ",
                "in case ",
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

                                                            if (original.Content.First() is XmlTextSyntax text)
                                                            {
                                                                return ReplaceText(original, text, Phrases, string.Empty);
                                                            }

                                                            return rewritten;
                                                        });

            return updatedSyntax;
        }
    }
}