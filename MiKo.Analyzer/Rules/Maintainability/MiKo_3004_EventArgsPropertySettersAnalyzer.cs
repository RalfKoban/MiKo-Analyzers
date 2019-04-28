using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3004_EventArgsPropertySettersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3004";

        public MiKo_3004_EventArgsPropertySettersAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol)
        {
            if (symbol.IsReadOnly)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var setter = symbol.SetMethod;
            if (setter == null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (setter.DeclaredAccessibility == Accessibility.Private)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (symbol.ContainingType?.IsEventArgs() == true)
            {
                return new[] { Issue(setter) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}