using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2024_CodeFixProvider)), Shared]
    public sealed class MiKo_2024_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2024_EnumParamDefaultPhraseAnalyzer.Id;

        protected override string Title => "Fix comment of Enum parameter";

        protected override XmlElementSyntax Comment(XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            return StartCommentWith(comment, Constants.Comments.EnumParameterStartingPhrase[0]);
        }
    }
}