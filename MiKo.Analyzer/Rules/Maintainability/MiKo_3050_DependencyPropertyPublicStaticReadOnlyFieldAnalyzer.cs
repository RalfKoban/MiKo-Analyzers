using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3050_DependencyPropertyPublicStaticReadOnlyFieldAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3050";

        public MiKo_3050_DependencyPropertyPublicStaticReadOnlyFieldAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty();

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol) => symbol.IsStatic && symbol.IsReadOnly && symbol.DeclaredAccessibility == Accessibility.Public
                                                                                       ? Enumerable.Empty<Diagnostic>()
                                                                                       : new[] { ReportIssue(symbol) };
    }
}