using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class OverallDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected OverallDocumentationAnalyzer(string id) : base(id, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.TypeParameter);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsPrimaryConstructor() is false; // records are analyzed for their type as well, so we do not need to report twice

        protected IEnumerable<Diagnostic> AnalyzeForSpecialPhrase(SyntaxToken token, string startingPhrase, Func<string, string> replacementCallback)
        {
            foreach (var location in GetAllLocations(token, startingPhrase))
            {
                var start = location.SourceSpan.Start;
                var index = start - token.SpanStart + startingPhrase.Length;

                var textAfterStartingPhrase = token.ValueText.AsSpan(index);
                var nextWord = textAfterStartingPhrase.FirstWord();

                // let's find the end of the next word in the source code (but keep in mind the offset of the starting phrase)
                var offset = start + startingPhrase.Length;
                var end = textAfterStartingPhrase.IndexOf(nextWord, StringComparison.Ordinal) + nextWord.Length + offset;

                var finalLocation = CreateLocation(token, start, end);
                var replacement = replacementCallback(nextWord.ToString());

                yield return Issue(finalLocation, replacement);
            }
        }

        protected virtual Diagnostic Issue(Location location, string replacement) => base.Issue(location, replacement);
    }
}