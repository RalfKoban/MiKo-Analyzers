using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6059_CodeFixProvider)), Shared]
    public sealed class MiKo_6059_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6059";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var token = syntax.FindToken(issue);

            var spaces = GetProposedSpaces(issue);

            return syntax.ReplaceToken(token, token.WithLeadingSpaces(spaces));
        }
    }
}