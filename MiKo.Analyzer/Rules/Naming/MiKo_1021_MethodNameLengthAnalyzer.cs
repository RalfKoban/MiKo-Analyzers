using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1021_MethodNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1021";

        public MiKo_1021_MethodNameLengthAnalyzer() : base(Id, SymbolKind.Method, 25)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol)
        {
            switch (symbol.MethodKind)
            {
                case MethodKind.EventAdd:
                case MethodKind.EventRemove:
                case MethodKind.ExplicitInterfaceImplementation:
                case MethodKind.PropertyGet:
                case MethodKind.PropertySet:
                    return Enumerable.Empty<Diagnostic>();
            }

            if (symbol.IsTestMethod()) return Enumerable.Empty<Diagnostic>();

            return Analyze(symbol);
        }
    }
}