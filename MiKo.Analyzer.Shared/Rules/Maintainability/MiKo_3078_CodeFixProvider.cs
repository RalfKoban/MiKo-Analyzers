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

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<EnumMemberDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is EnumMemberDeclarationSyntax member)
            {
                var isFlags = bool.Parse(issue.Properties[Constants.AnalyzerCodeFixSharedData.IsFlagged]);
                var position = int.Parse(issue.Properties[Constants.AnalyzerCodeFixSharedData.Position]);

                ExpressionSyntax expression;

                if (isFlags && position > 0)
                {
                    expression = SyntaxFactory.BinaryExpression(SyntaxKind.LeftShiftExpression, Literal(1), Literal(position - 1));
                }
                else
                {
                    expression = Literal(position);
                }

                return member.WithEqualsValue(SyntaxFactory.EqualsValueClause(expression));
            }

            return syntax;
        }
    }
}