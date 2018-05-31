using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2048_WillBePhraseAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2048";

        private static readonly string[] WillBePhrases = CreateWillBePhrases(" will be").ToArray();

        public MiKo_2048_WillBePhraseAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.NamedType, SymbolKind.Property);

        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => AnalyzeComment(symbol); // TODO: RKN refactor to use code from base class

        private IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol)
        {
            var comment = GetComment(symbol);

            return comment.ContainsAny(WillBePhrases)
                       ? new[] { ReportIssue(symbol) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static IEnumerable<string> CreateWillBePhrases(params string[] forbiddenTerms) => from suffix in Constants.Comments.Delimiters from forbiddenTerm in forbiddenTerms select forbiddenTerm + suffix;
    }
}