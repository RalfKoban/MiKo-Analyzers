using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4007_CodeFixProvider)), Shared]
    public sealed class MiKo_4007_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_4007";

        protected override Task<SyntaxNode> GetUpdatedTypeSyntaxAsync(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            // place before all other methods
            var updatedSyntax = PlaceFirst<MethodDeclarationSyntax>(syntax, typeSyntax);

            return Task.FromResult(updatedSyntax);
        }
    }
}