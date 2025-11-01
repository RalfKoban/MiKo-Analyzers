using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_1062_IsDetectionNameAnalyzer"/>
    /// <seealso cref="MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer"/>
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
                                                                         "IsAllLowerCase",
                                                                         "IsAllUpperCase",
                                                                         "IsAnyKind",
                                                                         "IsAssignableFrom",
                                                                         "IsAssignableTo",
                                                                         "IsByteArray",
                                                                         "IsCancellationToken",
                                                                         "IsCompleted",
                                                                         "IsDigitsOnly",
                                                                         "IsDragSource",
                                                                         "IsDropTarget",
                                                                         "IsEmptyArray",
                                                                         "IsLowerCase",
                                                                         "IsLowerCaseLetter",
                                                                         "IsNameOf",
                                                                         "IsNavigationTarget",
                                                                         "IsNotCompleted",
                                                                         "IsOfName",
                                                                         "IsPrimaryConstructor",
                                                                         "IsReadOnly",
                                                                         "IsReadWrite",
                                                                         "IsSingleWord",
                                                                         "IsSolutionWide",
                                                                         "IsTypeOf",
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
                                                                 "IsContainedIn",
                                                                 "IsDefault",
                                                                 "IsDroppedOver",
                                                                 "IsExcludedFrom",
                                                                 "IsFirst",
                                                                 "IsIn",
                                                                 "IsLast",
                                                                 "IsNext",
                                                                 "IsOfType",
                                                                 "IsPrevious",
                                                                 "IsSame",
                                                                 "IsShownAs",
                                                                 "IsShownIn",
                                                                 "IsTest",
                                                             };

        private static readonly string[] WellKnownPostfixes =
                                                              {
                                                                  "Child",
                                                                  "Code",
                                                                  "Line",
                                                                  "Mode",
                                                              };

        public MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            base.InitializeCore(context);

            InitializeCore(context, SymbolKind.Property);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsBoolean() && base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && symbol.GetReturnType()?.IsBoolean() is true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        private static bool HasIssue(string name) => name.Length > 5
                                                  && name.StartsWithAny(Prefixes)
                                                  && name.HasUpperCaseLettersAbove(2)
                                                  && name.StartsWithAny(WellKnownPrefixes) is false
                                                  && name.EndsWithAny(WellKnownPostfixes) is false
                                                  && WellKnownNames.Contains(name) is false;

        private Diagnostic[] AnalyzeName(ISymbol symbol) => HasIssue(symbol.Name)
                                                            ? new[] { Issue(symbol) }
                                                            : Array.Empty<Diagnostic>();
    }
}