using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2015_FireMethodsAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2015";

        private static readonly string[] AllowedWords = { "raise", "throw" };
        private static readonly string[] ForbiddenWords = { "fire", "fired", "fires", "firing" };

        private static readonly string AllowedWordsForRule = AllowedWords.HumanizedConcatenated();
        private static readonly string ForbiddenWordsForRule = ForbiddenWords.HumanizedConcatenated();

        private static readonly string[] ForbiddenPhrases = ForbiddenWords.WithDelimiters();

        public MiKo_2015_FireMethodsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                foreach (var location in GetAllLocations(token, ForbiddenPhrases, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(symbol.Name, location, AllowedWordsForRule, ForbiddenWordsForRule);
                }
            }
        }
    }
}