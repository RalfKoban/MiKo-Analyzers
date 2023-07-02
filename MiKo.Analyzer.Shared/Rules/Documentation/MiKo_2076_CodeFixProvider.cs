using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2076_CodeFixProvider)), Shared]
    public sealed class MiKo_2076_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2076_OptionalParameterDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2076_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlElementSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax element)
            {
                var onSameLine = element.StartTag.GetStartingLine() == element.EndTag.GetStartingLine();

                var startText = XmlText(onSameLine ? " " + Constants.Comments.DefaultStartingPhrase : Constants.Comments.DefaultStartingPhrase);
                var endText = XmlText(".");

                if (onSameLine)
                {
                    // no leading '///' to add because the text is located on the same line
                }
                else
                {
                    endText = endText.WithTrailingXmlComment();
                }

                var reference = GetDefaultValueReference(issue);

                return element.AddContent(startText, reference, endText);
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }

        private static XmlNodeSyntax GetDefaultValueReference(Diagnostic issue)
        {
            if (issue.Properties.TryGetValue(MiKo_2076_OptionalParameterDefaultPhraseAnalyzer.DefaultSeeLangwordValue, out var defaultValue))
            {
                return SeeLangword(defaultValue);
            }

            if (issue.Properties.TryGetValue(MiKo_2076_OptionalParameterDefaultPhraseAnalyzer.DefaultSeeCrefValue, out var defaultCrefValue))
            {
                return SeeCref(defaultCrefValue);
            }

            if (issue.Properties.TryGetValue(MiKo_2076_OptionalParameterDefaultPhraseAnalyzer.DefaultCodeValue, out var defaultCodeValue))
            {
                return C(defaultCodeValue);
            }

            return XmlText("TODO");
        }
    }
}