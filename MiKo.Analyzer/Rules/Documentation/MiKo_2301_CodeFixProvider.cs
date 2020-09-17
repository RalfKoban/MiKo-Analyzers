using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2301_CodeFixProvider)), Shared]
    public sealed class MiKo_2301_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2301_TestArrangeActAssertCommentAnalyzer.Id;

        protected override string Title => Resources.MiKo_2301_CodeFixTitle;

        protected override bool IsTrivia => true;

        protected override SyntaxToken GetUpdatedToken(SyntaxToken token)
        {
            var trivia = token.LeadingTrivia;
            var count = trivia.Count;

            if (token.GetPreviousToken().IsKind(SyntaxKind.OpenBraceToken))
            {
                // do not re-use the new line, to avoid gap between code and opening brace
                return token.WithLeadingTrivia(trivia[count - 1]);
            }

            return token.WithLeadingTrivia(trivia[count - 2], trivia[count - 1]);
        }
    }
}