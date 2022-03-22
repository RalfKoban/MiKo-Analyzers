using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3112_CodeFixProvider)), Shared]
    public class MiKo_3112_CodeFixProvider : UnitTestCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_3112_TestAssertsUseIsEmptyInsteadOfHasCountZeroAnalyzer.Id;

        protected sealed override string Title => Resources.MiKo_3112_CodeFixTitle;

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().First(MiKo_3112_TestAssertsUseIsEmptyInsteadOfHasCountZeroAnalyzer.HasIssue);

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var argumentSyntax = syntax.ToString().Contains(".Not.")
                                   ? Is("Not", "Empty")
                                   : Is("Empty");

            return argumentSyntax.Expression;
        }
    }
}