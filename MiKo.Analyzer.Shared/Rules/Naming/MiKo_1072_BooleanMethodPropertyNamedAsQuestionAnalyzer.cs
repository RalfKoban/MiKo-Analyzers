using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    //// <seealso cref="MiKo_1062_IsDetectionNameAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1072";

        private static readonly string[] Prefixes =
                                                    {
                                                        "Is",
                                                        "Are",
                                                    };

        private static readonly HashSet<string> WellKnownNames = new HashSet<string>
                                                                     {
                                                                         nameof(string.IsNullOrEmpty),
                                                                         nameof(string.IsNullOrWhiteSpace),
                                                                         "IsCompleted",
                                                                         "IsDigitsOnly",
                                                                         "IsDragSource",
                                                                         "IsDropTarget",
                                                                         "IsLowerCase",
                                                                         "IsLowerCaseLetter",
                                                                         "IsNavigationTarget",
                                                                         "IsNotCompleted",
                                                                         "IsReadOnly",
                                                                         "IsReadWrite",
                                                                         "IsSolutionWide",
                                                                         "IsUpperCase",
                                                                         "IsUpperCaseLetter",
                                                                         "IsValueConverter",
                                                                         "IsWhiteSpace",
                                                                         "IsWhiteSpaceOnly",
                                                                         "IsWriteProtected",
                                                                         "IsZipFile",
                                                                     };

        private static readonly string[] WellKnownPrefixes =
                                                             {
                                                                 "IsDefault",
                                                                 "IsInDesign",
                                                                 "IsOfType",
                                                                 "IsSame",
                                                                 "IsShownAs",
                                                                 "IsShownIn",
                                                                 "IsTest",
                                                             };

        private static readonly string[] WellKnownPostfixes =
                                                              {
                                                                  "Code",
                                                                  "Line",
                                                              };

        public MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            base.InitializeCore(context);

            InitializeCore(context, SymbolKind.Property);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.ReturnType.IsBoolean();

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && symbol.GetReturnType()?.IsBoolean() is true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var name = symbol.Name;

            if (name.Length <= 5)
            {
                // skip all short names (such as isIP)
                return Enumerable.Empty<Diagnostic>();
            }

            if (name.StartsWithAny(Prefixes, StringComparison.Ordinal) && name.HasUpperCaseLettersAbove(2))
            {
                if (WellKnownNames.Contains(name) || name.StartsWithAny(WellKnownPrefixes, StringComparison.Ordinal) || name.EndsWithAny(WellKnownPostfixes, StringComparison.Ordinal))
                {
                    // skip all well known names
                    return Enumerable.Empty<Diagnostic>();
                }

                return new[] { Issue(symbol) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}