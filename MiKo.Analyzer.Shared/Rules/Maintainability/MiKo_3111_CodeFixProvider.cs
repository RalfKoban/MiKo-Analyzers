using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3111_CodeFixProvider)), Shared]
    public class MiKo_3111_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_3111_TestAssertsUseZeroInsteadOfEqualToAnalyzer.Id;

        protected sealed override string Title => Resources.MiKo_3111_CodeFixTitle;

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().First();

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var node = (InvocationExpressionSyntax)syntax;

            if (node.Expression is MemberAccessExpressionSyntax m)
            {
                return SimpleMemberAccess(m.Expression, "Zero");
            }

            return node;
        }
    }
}