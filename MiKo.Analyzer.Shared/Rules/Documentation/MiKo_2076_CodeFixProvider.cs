using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2076_CodeFixProvider)), Shared]
    public sealed class MiKo_2076_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2076";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlElementSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax element, Diagnostic issue)
        {
            XmlTextSyntax startText;
            XmlTextSyntax endText;

            if (element.StartTag.IsOnSameLineAs(element.EndTag))
            {
                startText = XmlText(" " + Constants.Comments.DefaultStartingPhrase);

                // no trailing '///' to add because the text is located on the same line
                endText = XmlText(".");
            }
            else
            {
                startText = XmlText(Constants.Comments.DefaultStartingPhrase);

                endText = XmlText(".").WithTrailingXmlComment();
            }

            var reference = GetDefaultValueReference(issue);

            return element.AddContent(startText, reference, endText);
        }

        private static XmlNodeSyntax GetDefaultValueReference(Diagnostic issue)
        {
            if (issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.DefaultSeeLangwordValue, out var defaultValue))
            {
                return SeeLangword(defaultValue);
            }

            if (issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, out var defaultCrefValue))
            {
                if (defaultCrefValue != null)
                {
                    var dotIndex = defaultCrefValue.LastIndexOf('.');

                    if (dotIndex is -1)
                    {
                        return SeeCref(defaultCrefValue);
                    }

                    return SeeCref(defaultCrefValue.Substring(0, dotIndex), defaultCrefValue.Substring(dotIndex + 1));
                }
            }

            if (issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, out var defaultCodeValue))
            {
                return C(defaultCodeValue);
            }

            return XmlText(Constants.TODO);
        }
    }
}