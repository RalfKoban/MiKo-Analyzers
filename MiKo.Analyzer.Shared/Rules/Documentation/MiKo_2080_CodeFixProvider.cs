using System.Composition;

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

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var comment = (XmlElementSyntax)syntax;
            var fieldDeclaration = comment.FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var field = fieldDeclaration?.Declaration.Variables.First();

            var symbol = (IFieldSymbol)GetSymbol(document, field);
            var phrase = MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.GetStartingPhrase(symbol);

            return CommentStartingWith(comment, phrase);
        }
    }
}