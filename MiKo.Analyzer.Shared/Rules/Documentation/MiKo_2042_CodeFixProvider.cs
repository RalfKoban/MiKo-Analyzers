using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2042_CodeFixProvider)), Shared]
    public sealed class MiKo_2042_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly string[] Tags = { "br", "p" };

        public override string FixableDiagnosticId => "MiKo_2042";

        protected override Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntax(DocumentationCommentTriviaSyntax syntax)
        {
            foreach (var node in syntax.DescendantNodes())
            {
                switch (node)
                {
                    case XmlEmptyElementSyntax ee when Tags.Contains(ee.GetName()):
                        return syntax.ReplaceNode(ee, Para());

                    case XmlElementSyntax e when Tags.Contains(e.GetName()):
                        return syntax.ReplaceNode(e, Para(e.Content));
                }
            }

            return syntax;
        }
    }
}