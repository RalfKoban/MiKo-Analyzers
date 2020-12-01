using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    /// <seealso cref="MiKo_1062_IsDetectionNameAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1072";

        private static readonly string[] Prefixes =
            {
                "Is",
                "Are",
            };

        private static readonly string[] AllowedNames =
            {
                nameof(string.IsNullOrEmpty),
                nameof(string.IsNullOrWhiteSpace),
                "IsReadOnly",
                "IsReadWrite",
                "IsWriteProtected",
            };

        public MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            base.InitializeCore(context);

            InitializeCore(context, SymbolKind.Property);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.ReturnType.IsBoolean();

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && symbol.GetReturnType()?.IsBoolean() is true;

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
                // skip all well known names
                if (name.StartsWith("IsSame", StringComparison.Ordinal) || AllowedNames.Contains(name))
                {
                    yield break;
                }

                yield return Issue(symbol);
            }
        }
    }
}