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
        public override string FixableDiagnosticId => MiKo_2051_ExceptionTagDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2051_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
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

                                                            if (original.IsExceptionCommentFor<ArgumentOutOfRangeException>())
                                                            {
                                                                return GetFixedExceptionCommentForArgumentOutOfRangeException(original);
                                                            }

                                                            if (original.IsExceptionCommentFor<ArgumentException>())
                                                            {
                                                                return GetFixedExceptionCommentForArgumentException(original);
                                                            }

                                                            return GetFixedStartingPhrase(original);
                                                        });

            return updatedSyntax;
        }
    }
}