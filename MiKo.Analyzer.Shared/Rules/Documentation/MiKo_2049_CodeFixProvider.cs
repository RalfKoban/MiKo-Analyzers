using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2049_CodeFixProvider)), Shared]
    public sealed class MiKo_2049_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2049_WillBePhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2049_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return Comment(syntax, ReplacementMap.Keys, ReplacementMap);
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var pair in MiKo_2049_WillBePhraseAnalyzer.PhrasesMap)
            {
                dictionary.Add(pair.Key, pair.Value);
                dictionary.Add(pair.Key.ToUpperCaseAt(0), pair.Value.ToUpperCaseAt(0));
            }

            return dictionary;
        }
    }
}