using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeName(symbol);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            var containsDelete = symbolName.Contains(Delet, StringComparison.OrdinalIgnoreCase);
            var containsRemove = symbolName.Contains(Remov, StringComparison.OrdinalIgnoreCase);

            if (containsDelete && containsRemove)
            {
                // name contains both, such as in "DeleteRemovedStuff" or "RemoveDeletedStuff", so nothing to do
                return Enumerable.Empty<Diagnostic>();
            }

            if (containsDelete)
            {
                return AnalyzeDocumentation(symbol, Remov);
            }

            if (containsRemove)
            {
                return AnalyzeDocumentation(symbol, Delet);
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeDocumentation(ISymbol symbol, string forbiddenWord)
        {
            var documentation = symbol.GetDocumentationCommentTriviaSyntax();
            if (documentation is null)
            {
                // no documentation, so nothing to do
                return Enumerable.Empty<Diagnostic>();
            }

            var texts = documentation.DescendantNodes().OfType<XmlTextSyntax>().SelectMany(_ => _.TextTokens).Select(_ => _.ValueText);

            if (texts.Any(_ => _.Contains(forbiddenWord, StringComparison.OrdinalIgnoreCase)))
            {
                return new[] { Issue(symbol) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}