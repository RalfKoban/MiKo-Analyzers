using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2025_CodeFixProvider)), Shared]
    public class MiKo_2025_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_2025_CancellationTokenParamDefaultPhraseAnalyzer.Id;

        protected sealed override string Title => "Fix comment of CancellationToken";

        protected override XmlElementSyntax Comment(XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            return Comment(comment, Constants.Comments.CancellationTokenParameterPhrase);
        }
    }
}