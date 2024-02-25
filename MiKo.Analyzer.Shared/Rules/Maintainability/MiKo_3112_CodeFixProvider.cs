using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3112_CodeFixProvider)), Shared]
    public sealed class MiKo_3112_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3112";

        protected override string Title => Resources.MiKo_3112_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().FirstOrDefault(MiKo_3112_TestAssertsUseIsEmptyInsteadOfHasCountZeroAnalyzer.HasIssue);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var argumentSyntax = syntax.ToString().Contains(".Not.")
                                 ? Is("Not", "Empty")
                                 : Is("Empty");

            return argumentSyntax.Expression;
        }
    }
}