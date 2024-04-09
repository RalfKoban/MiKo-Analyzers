using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2200_CodeFixProvider)), Shared]
    public sealed class MiKo_2200_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2200";

        protected override string Title => Resources.MiKo_2200_CodeFixTitle;

        protected override bool IsTrivia => true;

        protected override SyntaxToken GetToken(SyntaxTrivia trivia, Diagnostic issue)
        {
            // we have the trivia, but we need the specific text token, so search for it
            var location = issue.Location;

            var syntaxNode = trivia.GetStructure();

            var token = syntaxNode.DescendantNodes<XmlTextSyntax>().Where(_ => _.GetLocation().Contains(location))
                                  .Select(_ => _.TextTokens.First(__ => __.GetLocation().Contains(location)))
                                  .First();

            return token;
        }

        protected override SyntaxToken GetUpdatedToken(SyntaxToken token, Diagnostic issue)
        {
            var text = token.Text;

            var index = 0;

            while (text[index].IsWhiteSpace())
            {
                index++;
            }

            var fixedText = text.ToUpperCaseAt(index);

            return token.WithText(fixedText);
        }
    }
}