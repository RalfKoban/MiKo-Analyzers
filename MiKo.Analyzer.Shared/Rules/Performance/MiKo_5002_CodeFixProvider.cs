using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5002_CodeFixProvider)), Shared]
    public sealed class MiKo_5002_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_5002";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().FirstOrDefault()?.Name;

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((SimpleNameSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static IdentifierNameSyntax GetUpdatedSyntax(SimpleNameSyntax identifier)
        {
            var name = identifier.GetName().WithoutSuffix("Format");

            return IdentifierName(name);
        }
    }
}