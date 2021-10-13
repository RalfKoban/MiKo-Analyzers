using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2025_CodeFixProvider)), Shared]
    public sealed class MiKo_2025_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2025_CancellationTokenParamDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2025_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax Comment(Document document, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic) => comment; // TODO RKN: fix

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            return Comment(comment, Constants.Comments.CancellationTokenParameterPhrase);
        }
    }
}