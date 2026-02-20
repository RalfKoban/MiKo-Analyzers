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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3044_CodeFixProvider)), Shared]
    public sealed class MiKo_3044_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3044";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LiteralExpressionSyntax>().FirstOrDefault();

        protected override async Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var literal = (LiteralExpressionSyntax)syntax;

            var type = await FindRelatedTypeAsync(literal, document, cancellationToken).ConfigureAwait(false);

            return type is null
                   ? NameOf(literal)
                   : NameOf(type, literal);
        }

        private static async Task<ITypeSymbol> FindRelatedTypeAsync(LiteralExpressionSyntax syntax, Document document, CancellationToken cancellationToken)
        {
            var identifierName = syntax.Token.ValueText;

            var ifStatement = syntax.GetEnclosing<IfStatementSyntax>();

            if (ifStatement != null)
            {
                // search if block
                return await FindRelatedTypeAsync(ifStatement.Statement, identifierName, document, cancellationToken).ConfigureAwait(false)
                    ?? await FindRelatedTypeAsync(ifStatement.Else, identifierName, document, cancellationToken).ConfigureAwait(false);
            }

            var switchStatement = syntax.GetEnclosing<SwitchStatementSyntax>();

            if (switchStatement != null)
            {
                // search switch block
                return await FindRelatedTypeAsync(switchStatement, identifierName, document, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        private static async Task<ITypeSymbol> FindRelatedTypeAsync(SyntaxNode syntax, string identifierName, Document document, CancellationToken cancellationToken)
        {
            var identifier = syntax?.FirstDescendant<IdentifierNameSyntax>(_ => _.GetName() == identifierName);

            if (identifier?.Parent is MemberAccessExpressionSyntax maes)
            {
                return await maes.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);
            }

            // try to find type
            return null;
        }
    }
}