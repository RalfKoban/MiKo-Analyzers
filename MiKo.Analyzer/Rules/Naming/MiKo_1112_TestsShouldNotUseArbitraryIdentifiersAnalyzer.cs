﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1112";

        internal const string Phrase = "arbitrary";

        public MiKo_1112_TestsShouldNotUseArbitraryIdentifiersAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(ISymbol symbol)
        {
            var betterName = symbol.Name.Without("Arbitrary");
            var i = betterName.IndexOf(Phrase, StringComparison.Ordinal);
            if (i < 0)
            {
                return betterName;
            }

            var characters = betterName.Without(Phrase).ToCharArray();
            if (characters.Length != 0)
            {
                characters[i] = char.ToLowerInvariant(characters[i]);

                return string.Intern(new string(characters));
            }

            // we cannot find a better name
            return Phrase;
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            base.InitializeCore(context);

            InitializeCore(context, SymbolKind.Method, SymbolKind.Property, SymbolKind.Field, SymbolKind.Parameter);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && symbol.ContainingType.IsTestClass();

        protected override bool ShallAnalyze(IFieldSymbol symbol) => base.ShallAnalyze(symbol) && symbol.ContainingType.IsTestClass();

        protected override bool ShallAnalyze(IParameterSymbol symbol) => base.ShallAnalyze(symbol) && ShallAnalyze(symbol.GetEnclosingMethod());

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                if (HasIssue(identifier.ValueText))
                {
                    var method = identifier.Parent.GetEnclosingMethod(semanticModel);

                    if (method?.IsTestMethod() is true)
                    {
                        var symbol = identifier.GetSymbol(semanticModel);

                        yield return Issue(symbol);
                    }
                }
            }
        }

        private static bool HasIssue(string name) => name?.Contains(Phrase, StringComparison.OrdinalIgnoreCase) is true;

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol) => HasIssue(symbol.Name)
                                                                           ? new[] { Issue(symbol) }
                                                                           : Enumerable.Empty<Diagnostic>();
    }
}