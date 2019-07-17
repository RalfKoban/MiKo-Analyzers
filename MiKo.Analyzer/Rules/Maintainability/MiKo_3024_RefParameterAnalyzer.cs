using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3024_RefParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3024";

        public MiKo_3024_RefParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            foreach (var parameter in method.Parameters.Where(_ => _.RefKind == RefKind.Ref && _.Type.TypeKind != TypeKind.Struct))
            {
                var syntax = (ParameterSyntax)parameter.DeclaringSyntaxReferences[0].GetSyntax();
                var refKeyword = syntax.Modifiers.First(_ => _.IsKind(SyntaxKind.RefKeyword));

                yield return Issue(parameter.Name, refKeyword.GetLocation());
            }
        }
    }
}