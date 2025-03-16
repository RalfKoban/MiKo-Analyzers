using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2002_CodeFixProvider)), Shared]
    public sealed class MiKo_2002_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2002";

        internal static SyntaxNode GetUpdatedSyntax(XmlElementSyntax comment)
        {
            var cref = comment.Content.LastOrDefault(_ => _.IsSeeCref()) ?? SeeCref(Constants.TODO);

            return Comment(comment, Constants.Comments.EventArgsSummaryStartingPhrase, cref, " event.");
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax((XmlElementSyntax)syntax);
    }
}