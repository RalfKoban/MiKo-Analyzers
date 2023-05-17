using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3103";

        private const string Invocation = nameof(Guid) + "." + nameof(Guid.NewGuid);

        public MiKo_3103_TestMethodsDoNotUseGuidNewGuidAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Field);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod() || symbol.ContainingType.IsTestClass();

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.ContainingType.IsTestClass();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => Analyze(symbol);

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation) => Analyze(symbol);

        private IEnumerable<Diagnostic> Analyze(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            return symbol.GetSyntax()
                         .DescendantNodes<MemberAccessExpressionSyntax>(_ => _.ToCleanedUpString() == Invocation)
                         .Select(_ => Issue(symbolName, _));
        }
    }
}