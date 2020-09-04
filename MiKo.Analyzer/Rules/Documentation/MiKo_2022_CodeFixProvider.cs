using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2022_CodeFixProvider)), Shared]
    public sealed class MiKo_2022_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2022_OutParamDefaultPhraseAnalyzer.Id;

        protected override string Title => "Fix comment start of [out] parameter";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            var symbol = (IParameterSymbol)GetSymbol(document, parameter);
            var phrase = MiKo_2022_OutParamDefaultPhraseAnalyzer.GetStartingPhrase(symbol);

            return CommentStartingWith(comment, phrase[0]);
        }
    }
}