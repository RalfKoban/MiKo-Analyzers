using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1080_UseNumbersInsteadOfWordingAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1080";

        private const string NumberOne = "one";

        private static readonly string[] NumbersWithoutOne =
            {
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

        private static readonly HashSet<char> NumberOneAllowedPreceedings = new HashSet<char>("bcdghlmnstxzBCDGHLMNSTXZ");

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

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.Text;

                if (HasIssue(name))
                {
                    yield return Issue(name, identifier.GetLocation());
                }
            }
        }

        private static bool HasIssue(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            // special handling for name 'one'
            var index = name.IndexOf(NumberOne, comparison);
            if (index == 0)
            {
                return true;
            }

            if (index > 0)
            {
                var charBeforeOne = name[index - 1];

                switch (name[index])
                {
                    case 'o':
                    {
                        if (NumberOneAllowedPreceedings.Contains(charBeforeOne) is false)
                        {
                            return true;
                        }

                        break;
                    }

                    case 'O':
                    {
                        if (charBeforeOne.IsUpperCase())
                        {
                            return true; // its a new word
                        }

                        if (NumberOneAllowedPreceedings.Contains(charBeforeOne) is false)
                        {
                            return true;
                        }

                        break;
                    }
                }
            }

            return name.ContainsAny(NumbersWithoutOne, comparison);
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