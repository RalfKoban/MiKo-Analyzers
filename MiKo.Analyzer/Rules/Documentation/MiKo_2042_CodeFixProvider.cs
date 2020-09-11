using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2042_CodeFixProvider)), Shared]
    public sealed class MiKo_2042_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2042_BrParaAnalyzer.Id;

        protected override string Title => Resources.MiKo_2042_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var d = (DocumentationCommentTriviaSyntax)syntax;

            foreach (var node in d.DescendantNodes())
            {
                switch (node)
                {
                    case XmlEmptyElementSyntax br when br.Name.LocalName.ValueText == "br":
                        d = d.ReplaceNode(br, Para());
                        break;

                    case XmlElementSyntax p when p.StartTag.Name.LocalName.ValueText == "p":
                        d = d.ReplaceNode(p, Para(p.Content));
                        break;
                }
            }

            return d;
        }
    }
}