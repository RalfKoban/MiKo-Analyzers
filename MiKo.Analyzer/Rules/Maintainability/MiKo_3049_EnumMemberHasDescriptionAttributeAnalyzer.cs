using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3049_EnumMemberHasDescriptionAttributeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3049";

        private static readonly string DescriptionAttributeName = typeof(System.ComponentModel.DescriptionAttribute).FullName;

        public MiKo_3049_EnumMemberHasDescriptionAttributeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEnum();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol)
        {
            var issues = symbol.GetMembers().OfType<IFieldSymbol>()
                               .Where(_ => _.HasAttributeApplied(DescriptionAttributeName) is false)
                               .Select(_ => Issue(_));
            return issues;
        }
    }
}