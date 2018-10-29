using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3041_EventArgsDelegateParametersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3041";

        public MiKo_3041_EventArgsDelegateParametersAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method) => method.ContainingType.IsEventArgs()
                                                                                              ? AnalyzeEventArgsMethod(method)
                                                                                              : Enumerable.Empty<Diagnostic>();

        private IEnumerable<Diagnostic> AnalyzeEventArgsMethod(IMethodSymbol method)
        {
            switch (method.MethodKind)
            {
                case MethodKind.Ordinary when method.IsOverride || method.IsInterfaceImplementation():
                    return Enumerable.Empty<Diagnostic>();

                case MethodKind.Constructor:
                case MethodKind.Ordinary:
                case MethodKind.PropertySet:
                    return method.Parameters.Where(_ => _.Type.TypeKind == TypeKind.Delegate).Select(_ => ReportIssue(_)).ToList();

                case MethodKind.PropertyGet when method.ReturnType.TypeKind == TypeKind.Delegate:
                    return new[] { ReportIssue(method.ContainingSymbol) };


                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }
    }
}