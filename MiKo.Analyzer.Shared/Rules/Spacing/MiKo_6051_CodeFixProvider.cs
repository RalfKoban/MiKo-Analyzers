using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6051_CodeFixProvider)), Shared]
    public sealed class MiKo_6051_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzer.Id;

        protected override string Title => Resources.MiKo_6051_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ConstructorInitializerSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ConstructorInitializerSyntax initializer)
            {
                var spaces = MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzer.GetSpaces(issue);

                var updatedToken = initializer.ColonToken.WithLeadingSpaces(spaces).WithTrailingSpace();
                var updatedKeyword = initializer.ThisOrBaseKeyword.WithoutLeadingTrivia();

                return initializer.WithColonToken(updatedToken)
                                  .WithThisOrBaseKeyword(updatedKeyword);
            }

            return syntax;
        }
    }
}