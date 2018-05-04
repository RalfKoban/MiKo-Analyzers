using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3040_BooleanMethodParametersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3040";

        public MiKo_3040_BooleanMethodParametersAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method) => method.MethodKind == MethodKind.Ordinary
                                                                                              ? method.Parameters
                                                                                                      .Where(_ => _.Type.SpecialType == SpecialType.System_Boolean)
                                                                                                      .SelectMany(_ => _.DeclaringSyntaxReferences)
                                                                                                      .Select(_ => _.GetSyntax())
                                                                                                      .OfType<ParameterSyntax>()
                                                                                                      .Select(_ => ReportIssue(_.Identifier.ValueText, _.Type.GetLocation()))
                                                                                              : Enumerable.Empty<Diagnostic>();
    }
}