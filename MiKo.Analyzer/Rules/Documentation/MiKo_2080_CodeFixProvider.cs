using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2080_CodeFixProvider)), Shared]
    public sealed class MiKo_2080_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2080_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;
            var fieldDeclaration = comment.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();
            var field = fieldDeclaration.Declaration.Variables.First();

            var symbol = (IFieldSymbol)GetSymbol(context, field);
            var phrase = MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.GetStartingPhrase(symbol);

            return CommentStartingWith(comment, phrase);
        }
    }
}