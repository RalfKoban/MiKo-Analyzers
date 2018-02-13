using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1011_DoMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1011";

        public MiKo_1011_DoMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsInterfaceImplementationOf<ICommand>()) return Enumerable.Empty<Diagnostic>();

            var methodName = method.Name;

            const StringComparison Comparison = StringComparison.Ordinal;

            var index = methodName.IndexOf("Do", Comparison);
            if (index == -1) return Enumerable.Empty<Diagnostic>();

            if (index == methodName.LastIndexOf("Do", Comparison) && methodName.ContainsAny("Dock", "Double", "Down", "Dot")) return Enumerable.Empty<Diagnostic>();

            return new[] { ReportIssue(method, methodName.Replace("Do", string.Empty)) };
        }
    }
}