using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2075_CodeFixProvider)), Shared]
    public sealed class MiKo_2075_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2075_ActionFunctionParameterPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2075_CodeFixTitle.FormatWith(MiKo_2075_ActionFunctionParameterPhraseAnalyzer.Replacement);

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, MiKo_2075_ActionFunctionParameterPhraseAnalyzer.Terms, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            return MiKo_2075_ActionFunctionParameterPhraseAnalyzer.Terms.ToDictionary(_ => _, _ => MiKo_2075_ActionFunctionParameterPhraseAnalyzer.Replacement);
        }
    }
}