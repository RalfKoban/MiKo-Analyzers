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
            if (methodName.IndexOf("Do", Comparison) == -1) return Enumerable.Empty<Diagnostic>();

            var escapedMethod = methodName
                                .Replace("Dock", "##ck")
                                .Replace("Document", "##cument")
                                .Replace("Does", "##es")
                                .Replace("Double", "##uble")
                                .Replace("Done", "##ne")
                                .Replace("Dot", "##t")
                                .Replace("Down", "##wn");
            if (escapedMethod.IndexOf("Do", Comparison) == -1) return Enumerable.Empty<Diagnostic>();

            return new[] { ReportIssue(method, escapedMethod.Replace("Do", string.Empty).Replace("##", "Do")) };
        }
    }
}