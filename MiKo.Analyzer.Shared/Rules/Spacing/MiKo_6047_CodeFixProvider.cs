using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6047_CodeFixProvider)), Shared]
    public sealed class MiKo_6047_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6047";

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedSyntax((SwitchExpressionSyntax)syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private static SwitchExpressionSyntax GetUpdatedSyntax(SwitchExpressionSyntax expression, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);
            var armSpaces = spaces + Constants.Indentation;

            return expression.WithOpenBraceToken(expression.OpenBraceToken.WithLeadingSpaces(spaces))
                             .WithArms(SyntaxFactory.SeparatedList(
                                                               expression.Arms.Select(_ => _.WithLeadingSpaces(armSpaces)),
                                                               expression.Arms.GetSeparators()))
                             .WithCloseBraceToken(expression.CloseBraceToken.WithLeadingSpaces(spaces));
        }
    }
}