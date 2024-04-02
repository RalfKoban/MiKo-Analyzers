using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2230_CodeFixProvider)), Shared]
    public sealed class MiKo_2230_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2230";

        protected override string Title => Resources.MiKo_2230_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var token = syntax.FindToken(diagnostic);

            // TODO RKN Apply list
            return syntax.ReplaceNode(token.Parent, UpdateText(token));
        }

        private static IEnumerable<XmlNodeSyntax> UpdateText(SyntaxToken token)
        {
            var text = token.Text.AsSpan();

            var index = text.IndexOf(Constants.Comments.ValueMeaningPhrase.AsSpan(), StringComparison.Ordinal);
            var remainingText = text.Slice(0, index).Trim();

            yield return XmlText(remainingText).WithLeadingXmlComment();

            /// <list type=""table"">
            /// <listheader><term>Value</term><description>Meaning</description></listheader>
            /// <item><term>Less than zero</term><description>Some text here.</description></item>
            /// <item><term>Zero</term><description>Some other text here.</description></item>
            /// <item><term>Greater than zero</term><description>Some even other text here.</description></item>
            /// </list>
            var term = XmlElement(Constants.XmlTag.Term, XmlText(Constants.Comments.ValuePhrase));
            var description = XmlElement(Constants.XmlTag.Description, XmlText(Constants.Comments.MeaningPhrase));

            var items = new List<XmlNodeSyntax>
                            {
                                XmlElement(Constants.XmlTag.ListHeader, new[] { term, description }),
                            };

            yield return XmlList(Constants.XmlTag.ListType.Table, items).WithTrailingXmlComment();
        }
    }
}