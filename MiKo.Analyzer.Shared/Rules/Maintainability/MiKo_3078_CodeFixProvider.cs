using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3078_CodeFixProvider)), Shared]
    public sealed class MiKo_3078_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3078";

        protected override string Title => Resources.MiKo_3078_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<EnumMemberDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is EnumMemberDeclarationSyntax member)
            {
                var position = MiKo_3078_EnumMembersHaveValueAnalyzer.GetPosition(issue);
                var isFlags = MiKo_3078_EnumMembersHaveValueAnalyzer.IsFlags(issue);

                var expression = isFlags && position > 0
                                 ? SyntaxFactory.BinaryExpression(SyntaxKind.LeftShiftExpression, Literal(1), Literal(position - 1))
                                 : (ExpressionSyntax)Literal(position);

                return member.WithEqualsValue(SyntaxFactory.EqualsValueClause(expression));
            }

            return syntax;
        }
    }
}