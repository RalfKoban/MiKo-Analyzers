using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2042_CodeFixProvider)), Shared]
    public sealed class MiKo_2042_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2042_BrParaAnalyzer.Id;

        protected override string Title => Resources.MiKo_2042_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            foreach (var node in syntax.DescendantNodes())
            {
                switch (node)
                {
                    case XmlEmptyElementSyntax br when br.GetName() == "br":
                        return syntax.ReplaceNode(br, Para());

                    case XmlElementSyntax p when p.GetName() == "p":
                        return syntax.ReplaceNode(p, Para(p.Content));
                }
            }

            return syntax;
        }
    }
}