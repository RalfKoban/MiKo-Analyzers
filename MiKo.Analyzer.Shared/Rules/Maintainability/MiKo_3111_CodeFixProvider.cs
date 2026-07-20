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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3111_CodeFixProvider)), Shared]
    public sealed class MiKo_3111_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3111";

        protected internal override string GetTitle(Diagnostic issue) => Resources.MiKo_3111_CodeFixTitle.FormatWith(GetReplacement(issue));

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            var node = (InvocationExpressionSyntax)syntax;

            if (node.Expression is MemberAccessExpressionSyntax m)
            {
                if (m.Expression.ToString() is "Has.Count")
                {
                    return MemberIs("Empty");
                }

                var replacement = GetReplacement(issue);

                return Member(m.Expression, replacement);
            }

            return node;
        }

        private static string GetReplacement(Diagnostic issue) => issue?.Properties[Constants.AnalyzerCodeFixSharedData.NUnitReplacement] ?? string.Empty;
    }
}