using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                                                       "first",
                                                       "second",
                                                       "third",
                                                       "fourth",
                                                       "fifth",
                                                       "ninth",
                                                       "tenth",
                                                   };

        private static readonly IEnumerable<string> KnownParts = new[]
                                                                     {
                                                                         "_one_",
                                                                         "_first_",
                                                                         "_second_",
                                                                         "_third_",
                                                                         "anyone",
                                                                         "Anyone",
                                                                         "bone",
                                                                         "Bone",
                                                                         "omponent", // 'component'
                                                                         "OMPONENT",
                                                                         "cone",
                                                                         "Cone",
                                                                         "done",
                                                                         "Done",
                                                                         "etwork", // 'network'
                                                                         "ETWORK",
                                                                         "everyone",
                                                                         "Everyone",
                                                                         "forType",
                                                                         "ForType",
                                                                         "gone",
                                                                         "Gone",
                                                                         "height",
                                                                         "Height",
                                                                         "HEIGHT",
                                                                         "hone",
                                                                         "Hone",
                                                                         "ione",
                                                                         "ionE",
                                                                         "IonE",
                                                                         "late", // prevent stuff like 'CalculateLevenshtein' which contains the term 'eLeven'
                                                                         "lone",
                                                                         "Lone",
                                                                         "mone",
                                                                         "Mone",
                                                                         "none",
                                                                         "None",
                                                                         "NONE",
                                                                         "noOne",
                                                                         "NoOne",
                                                                         "onE", // 'SetupNonExistentDevice'
                                                                         "OnE",
                                                                         "OneTime",
                                                                         "Ones",
                                                                         "oNeeded",
                                                                         "oxone",
                                                                         "Oxone",
                                                                         "sEven", // 'isEvent'
                                                                         "someone", // 'someone'
                                                                         "Someone", // 'someone'
                                                                         "sone",
                                                                         "Sone",
                                                                         "seconds",
                                                                         "Seconds",
                                                                         "tone",
                                                                         "Tone",
                                                                         "twoLetter", // 'twoLetterLanguageCode'
                                                                         "TwoLetter", // 'TwoLetterLanguageCode'
                                                                         "tName", // 'firstName'
                                                                         "tWord", // 'firstWord'
                                                                         "WaitOne",
                                                                         "WAITONE",
                                                                         "weight",
                                                                         "Weight",
                                                                         "WEIGHT",
                                                                         "work", // 'ImportWorkflow'
                                                                         "Work",
                                                                         "zone",
                                                                         "Zone",
                                                                         "ZONE",
                                                                         "xponent",
                                                                     };

        public MiKo_1080_UseNumbersInsteadOfWordingAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            base.InitializeCore(context);

            InitializeCore(context, SymbolKind.Namespace, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.Parameter);
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => from identifier in identifiers
                                                                                                                                                          let name = identifier.Text
                                                                                                                                                          where HasIssue(name)
                                                                                                                                                          select Issue(name, identifier);

        private static bool HasIssue(string name)
        {
            // double check for performance improvements (we do not need to replace if it's not contained at all)
            if (name.ContainsAny(Numbers, StringComparison.OrdinalIgnoreCase))
            {
                var nameToInspect = HandleKnownParts(name);

                return nameToInspect.ContainsAny(Numbers, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static string HandleKnownParts(string name)
        {
            var finalName = new StringBuilder(name);

            foreach (var part in KnownParts)
            {
                finalName.ReplaceWithCheck(part, "#");
            }

            return finalName.ToString();
        }

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol) => HasIssue(symbol.Name)
                                                                       ? new[] { Issue(symbol) }
                                                                       : Enumerable.Empty<Diagnostic>();
    }
}