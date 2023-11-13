using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var parameter in symbol.Parameters.Where(_ => _.RefKind == RefKind.Ref && _.Type.TypeKind != TypeKind.Struct))
            {
                var syntax = parameter.GetSyntax();

                var refKeyword = syntax.Modifiers.First(SyntaxKind.RefKeyword);

                yield return Issue(parameter.Name, refKeyword);
            }
        }
    }
}