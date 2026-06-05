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
            var invocationsCount = invocations.Count;

            if (invocationsCount > 0)
            {
                Dictionary<string, IParameterSymbol> parameterNames = null;

                for (var index = 0; index < invocationsCount; index++)
                {
                    if (invocations[index].Expression is MemberAccessExpressionSyntax m && IsCall(m))
                    {
                        if (parameterNames is null)
                        {
                            parameterNames = symbol.Parameters.Where(IsIEnumerable).ToDictionary(_ => _.Name);
                        }

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

        private static bool IsCall(MemberAccessExpressionSyntax syntax)
        {
            var name = syntax.GetName();

            return name is nameof(Enumerable.ToList) || name is nameof(Enumerable.ToArray);
        }

        private static bool IsIEnumerable(IParameterSymbol parameter) => parameter.Type.OriginalDefinition.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T;
    }
}