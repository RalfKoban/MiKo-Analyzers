using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3008_ListReturnValueAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3008";

        private static readonly Type[] ForbiddenTypes =
                                                        {
                                                            typeof(ICollection<>),
                                                            typeof(ICollection),
                                                        };

        public MiKo_3008_ListReturnValueAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Interface;

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation) => symbol.GetMethods(MethodKind.Ordinary).Select(AnalyzeOrdinaryMethod).WhereNotNull();

        private Diagnostic AnalyzeOrdinaryMethod(IMethodSymbol method)
        {
            if (method.ReturnsVoid)
            {
                return null;
            }

            var returnType = method.ReturnType;

            switch (returnType.SpecialType)
            {
                case SpecialType.System_Array:
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_ICollection_T:
                    return Issue(method);

                case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    return null;
            }

            return AnalyzeReturnType(method, returnType);
        }

        private Diagnostic AnalyzeReturnType(IMethodSymbol method, ITypeSymbol returnType)
        {
            var returnTypeString = returnType.ToString();

            if (returnTypeString == "byte[]")
            {
                return null;
            }

            if (ForbiddenTypes.Exists(returnType.ImplementsPotentialGeneric))
            {
                return IssueOnType(returnType, method);
            }

            return null;
        }
    }
}