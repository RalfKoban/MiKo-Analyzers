using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1072";

        private static readonly string[] Prefixes =
            {
                "Is",
                "Are",
            };

        public MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            base.InitializeCore(context);

            InitializeCore(context, SymbolKind.Property);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsBoolean();

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType()?.IsBoolean() == true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var name = symbol.Name;
            if (name.Length <= 5)
            {
                // skip all short names (such as isIP)
                yield break;
            }

            if (name.StartsWithAny(Prefixes, StringComparison.Ordinal) && name.HasUpperCaseLettersAbove(2))
            {
                yield return Issue(symbol);
            }
        }
    }
}