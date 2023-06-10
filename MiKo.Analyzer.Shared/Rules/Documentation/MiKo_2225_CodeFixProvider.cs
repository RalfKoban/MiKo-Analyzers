using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2225_CodeFixProvider)), Shared]
    public sealed class MiKo_2225_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2225_SpecialCodeTagCIsOnSameLineAnalyzer.Id;

        protected override string Title => Resources.MiKo_2225_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlElementSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax element && element.Content.FirstOrDefault() is XmlTextSyntax text)
            {
                var code = text.GetTextWithoutTrivia().ToString();

                return element.WithContent(XmlText(code));
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }
    }
}