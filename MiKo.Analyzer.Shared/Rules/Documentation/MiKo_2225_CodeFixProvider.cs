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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2225_CodeFixProvider)), Shared]
    public sealed class MiKo_2225_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2225";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlElementSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((XmlElementSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax GetUpdatedSyntax(XmlElementSyntax element)
        {
            if (element?.Content.FirstOrDefault() is XmlTextSyntax text)
            {
                var code = text.GetTextTrimmed();

                return element.WithContent(XmlText(code));
            }

            return null;
        }
    }
}