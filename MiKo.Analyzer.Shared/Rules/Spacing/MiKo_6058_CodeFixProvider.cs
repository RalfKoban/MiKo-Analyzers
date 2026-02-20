using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6058_CodeFixProvider)), Shared]
    public sealed class MiKo_6058_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6058";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<TypeParameterConstraintClauseSyntax>().First();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((TypeParameterConstraintClauseSyntax)syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private static TypeParameterConstraintClauseSyntax GetUpdatedSyntax(TypeParameterConstraintClauseSyntax node, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            return node.WithLeadingSpaces(spaces);
        }
    }
}