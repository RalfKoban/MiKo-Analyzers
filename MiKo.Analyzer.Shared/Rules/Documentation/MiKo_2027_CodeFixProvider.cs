using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2027_CodeFixProvider)), Shared]
    public sealed class MiKo_2027_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2027_SerializationCtorParamDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2027_CodeFixTitle;

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            var phrase = (index == 0)
                         ? Constants.Comments.CtorSerializationInfoParamPhrase
                         : Constants.Comments.CtorStreamingContextParamPhrase;

            return Comment(comment, phrase);
        }
    }
}