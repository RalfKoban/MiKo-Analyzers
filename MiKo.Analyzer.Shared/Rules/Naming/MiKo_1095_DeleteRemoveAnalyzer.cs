using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1095_DeleteRemoveAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1095";

        private const string Delet = "Delet"; // without 'e' to also identify words such as 'deletion'
        private const string Remov = "Remov"; // without 'e' to also identify words such as 'removal'

        public MiKo_1095_DeleteRemoveAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        private Diagnostic[] AnalyzeName(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            var containsDelete = symbolName.Contains(Delet, StringComparison.OrdinalIgnoreCase);
            var containsRemove = symbolName.Contains(Remov, StringComparison.OrdinalIgnoreCase);

            if (containsDelete && containsRemove)
            {
                // name contains both, such as in "DeleteRemovedStuff" or "RemoveDeletedStuff", so nothing to do
                return Array.Empty<Diagnostic>();
            }

            if (containsDelete)
            {
                return AnalyzeDocumentation(symbol, Remov);
            }

            if (containsRemove)
            {
                return AnalyzeDocumentation(symbol, Delet);
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeDocumentation(ISymbol symbol, string forbiddenWord)
        {
            var comments = symbol.GetDocumentationCommentTriviaSyntax();
            var commentsLength = comments.Length;

            if (commentsLength > 0)
            {
                for (var index = 0; index < commentsLength; index++)
                {
                    foreach (var token in comments[index].GetXmlTextTokens())
                    {
                        var text = token.ValueText;

                        if (text.Length >= forbiddenWord.Length && text.Contains(forbiddenWord, StringComparison.OrdinalIgnoreCase))
                        {
                            return new[] { Issue(symbol) };
                        }
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}