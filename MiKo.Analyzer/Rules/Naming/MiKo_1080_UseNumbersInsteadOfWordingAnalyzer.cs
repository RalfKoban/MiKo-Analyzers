using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1080_UseNumbersInsteadOfWordingAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1080";

        private static readonly string[] Numbers =
            {
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine",
                "eleven",
                "twelve",
                "thirteen",
                "fifteen",
                "twenty",
                "thirty",
                "forty",
                "fifty",
            };

        private static readonly IEnumerable<string> KnownParts = new[]
                                                                     {
                                                                         "_one_",
                                                                         "bone",
                                                                         "Bone",
                                                                         "cone",
                                                                         "Cone",
                                                                         "done",
                                                                         "Done",
                                                                         "gone",
                                                                         "Gone",
                                                                         "height",
                                                                         "Height",
                                                                         "hone",
                                                                         "Hone",
                                                                         "ionE",
                                                                         "IonE",
                                                                         "lone",
                                                                         "Lone",
                                                                         "mone",
                                                                         "Mone",
                                                                         "none",
                                                                         "None",
                                                                         "oxone",
                                                                         "Oxone",
                                                                         "sEven", // 'isEvent'
                                                                         "sone",
                                                                         "Sone",
                                                                         "tone",
                                                                         "Tone",
                                                                         "tWord", // 'firstWord'
                                                                         "weight",
                                                                         "Weight",
                                                                         "zone",
                                                                         "Zone",
                                                                     };

        public MiKo_1080_UseNumbersInsteadOfWordingAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            base.InitializeCore(context);

            InitializeCore(context, SymbolKind.Namespace, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.Parameter);
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => from identifier in identifiers
                                                                                                                                        let name = identifier.Text
                                                                                                                                        where HasIssue(name)
                                                                                                                                        select Issue(name, identifier.GetLocation());

        private static bool HasIssue(string name)
        {
            var nameToInspect = Prepare(name);

            return nameToInspect.ContainsAny(Numbers, StringComparison.OrdinalIgnoreCase);
        }

        private static string Prepare(string name)
        {
            var finalName = KnownParts.Aggregate(name, (current, part) => current.Replace(part, "#"));
            return finalName;
        }

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            if (HasIssue(symbol.Name))
            {
                yield return Issue(symbol);
            }
        }
    }
}