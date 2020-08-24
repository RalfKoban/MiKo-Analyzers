using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2042_CodeFixProvider)), Shared]
    public sealed class MiKo_2042_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2042_BrParaAnalyzer.Id;

        protected override string Title => "Replace <br/> with <para/>";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var d = (DocumentationCommentTriviaSyntax)syntax;

            foreach (var node in d.DescendantNodes())
            {
                switch (node)
                {
                    case XmlEmptyElementSyntax br when br.Name.LocalName.ValueText == "br":
                        d = d.ReplaceNode(br, SyntaxFactory.XmlEmptyElement(Constants.XmlTag.Para));
                        break;

                    case XmlElementSyntax p when p.StartTag.Name.LocalName.ValueText == "p":
                        d = d.ReplaceNode(p, SyntaxFactory.XmlElement(Constants.XmlTag.Para, p.Content));
                        break;
                }
            }

            return d;
        }
    }
}