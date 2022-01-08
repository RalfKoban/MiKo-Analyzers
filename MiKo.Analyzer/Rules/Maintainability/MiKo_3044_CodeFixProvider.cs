using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3044_CodeFixProvider)), Shared]
    public sealed class MiKo_3044_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3044_PropertyChangeEventArgsUsageUsesNameofAnalyzer.Id;

        protected override string Title => Resources.MiKo_3044_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LiteralExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var literal = (LiteralExpressionSyntax)syntax;
            var type = FindRelatedType(context, literal);

            return type is null
                       ? NameOf(literal)
                       : NameOf(type, literal);
        }

        private static ITypeSymbol FindRelatedType(CodeFixContext context, LiteralExpressionSyntax syntax)
        {
            var identifierName = syntax.Token.ValueText;

            var ifStatement = syntax.GetEnclosing<IfStatementSyntax>();
            if (ifStatement != null)
            {
                // search if block
                return FindRelatedType(context,  ifStatement.Statement, identifierName) ?? FindRelatedType(context, ifStatement.Else, identifierName);
            }

            var switchStatement = syntax.GetEnclosing<SwitchStatementSyntax>();
            if (switchStatement != null)
            {
                // search switch block
                return FindRelatedType(context, switchStatement, identifierName);
            }

            return null;
        }

        private static ITypeSymbol FindRelatedType(CodeFixContext context, SyntaxNode syntax, string identifierName)
        {
            var identifier = syntax?.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault(_ => _.GetName() == identifierName);

            if (identifier?.Parent is MemberAccessExpressionSyntax maes)
            {
                var semanticModel = GetSemanticModel(context);

                return maes.GetTypeSymbol(semanticModel);
            }

            // try to find type
            return null;
        }
    }
}