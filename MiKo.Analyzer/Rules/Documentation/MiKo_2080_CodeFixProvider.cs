using System.Composition;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2080_CodeFixProvider)), Shared]
    public sealed class MiKo_2080_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.Id;

        protected override string Title => "Start field with default phrase";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;
            var fieldDeclaration = comment.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();
            var field = fieldDeclaration.Declaration.Variables.First();

            var symbol = (IFieldSymbol)GetSymbol(document, field);
            var phrase = MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.GetStartingPhrase(symbol);

            return CommentStartingWith(comment, phrase);
        }
    }
}