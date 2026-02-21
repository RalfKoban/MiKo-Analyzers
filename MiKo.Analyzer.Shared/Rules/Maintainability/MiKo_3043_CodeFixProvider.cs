using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3043_CodeFixProvider)), Shared]
    public sealed class MiKo_3043_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3043";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var invocation = (InvocationExpressionSyntax)syntax;

            if (invocation.Expression is MemberAccessExpressionSyntax maes && maes.Expression is GenericNameSyntax generic)
            {
                var type = generic.TypeArgumentList.Arguments[0];

                var argument = invocation.ArgumentList.Arguments[1].Expression;
                var nameOf = NameOf(type, (LiteralExpressionSyntax)argument);

                return syntax.ReplaceNode(argument, nameOf);
            }

            return invocation;
        }
    }
}