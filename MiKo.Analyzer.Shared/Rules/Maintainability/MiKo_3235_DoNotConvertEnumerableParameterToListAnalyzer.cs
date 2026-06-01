using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3235_DoNotConvertEnumerableParameterToListAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3235";

        public MiKo_3235_DoNotConvertEnumerableParameterToListAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ContainingType?.TypeKind is TypeKind.Class && symbol.Parameters.Any(IsIEnumerable);

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var methodDeclaration = symbol.GetSyntax();

            var invocations = methodDeclaration.DescendantNodes<InvocationExpressionSyntax>(SyntaxKind.InvocationExpression);
            int invocationsCount = invocations.Count;

            if (invocationsCount > 0)
            {
                var parameterNames = symbol.Parameters.Where(IsIEnumerable).ToDictionary(_ => _.Name);

                for (var index = 0; index < invocationsCount; index++)
                {
                    var invocation = invocations[index];

                    if (invocation.Expression is MemberAccessExpressionSyntax m && m.GetName() is nameof(Enumerable.ToList))
                    {
                        if (parameterNames.TryGetValue(m.GetIdentifierName(), out var parameter))
                        {
                            var parameterSyntax = parameter.GetSyntax();

                            return new[] { Issue(parameterSyntax.Type) };
                        }
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool IsIEnumerable(IParameterSymbol parameter) => parameter.Type.OriginalDefinition.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T;
    }
}