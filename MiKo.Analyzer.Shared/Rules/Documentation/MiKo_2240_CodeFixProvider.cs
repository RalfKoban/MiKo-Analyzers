using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2240_CodeFixProvider)), Shared]
    public sealed class MiKo_2240_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2240";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlElementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax element)
            {
                var phrase = GetPhraseProposal(issue);

                return Comment(element, phrase);
            }

            return syntax;
        }
    }
}