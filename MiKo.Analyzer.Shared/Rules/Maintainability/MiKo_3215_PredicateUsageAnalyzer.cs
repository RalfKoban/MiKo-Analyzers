using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_3001_DelegateAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3215_PredicateUsageAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3215";

        public MiKo_3215_PredicateUsageAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var parameter in symbol.Parameters)
            {
                var type = parameter.Type;

                if (type.TypeKind == TypeKind.Delegate && type.Name == "Predicate" && type.TryGetGenericArgumentType(out var genericType))
                {
                    yield return Issue(parameter.GetSyntax<ParameterSyntax>().Type, genericType.Name);
                }
            }
        }
    }
}