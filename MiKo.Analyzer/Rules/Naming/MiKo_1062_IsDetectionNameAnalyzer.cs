using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    //// <seealso cref="MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer"/>
    //// <seealso cref="MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1062_IsDetectionNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1062";

        private static readonly string[] Prefixes = { "Can", "Has", "Contains" };

        public MiKo_1062_IsDetectionNameAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property, SymbolKind.Field);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsBoolean();

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType()?.IsBoolean() is true;

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsBoolean();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeCamelCase(symbol, symbol.Name, 4);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeCamelCase(symbol, symbol.Name, 3);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol)
        {
            var symbolName = symbol.Name;

            foreach (var prefix in Constants.Markers.FieldPrefixes.Where(_ => _.Length > 0).Where(_ => symbolName.StartsWith(_, StringComparison.OrdinalIgnoreCase)))
            {
                symbolName = symbolName.Substring(prefix.Length);
                break;
            }

            return AnalyzeCamelCase(symbol, symbolName, 2);
        }

        private static bool ViolatesLimit(string name, ushort limit) => name.StartsWithAny(Prefixes, StringComparison.OrdinalIgnoreCase) && name.HasUpperCaseLettersAbove(limit);

        private IEnumerable<Diagnostic> AnalyzeCamelCase(ISymbol symbol, string symbolName, ushort limit) => ViolatesLimit(symbolName, limit)
                                                                                                              ? new[] { Issue(symbol, limit) }
                                                                                                              : Enumerable.Empty<Diagnostic>();
    }
}