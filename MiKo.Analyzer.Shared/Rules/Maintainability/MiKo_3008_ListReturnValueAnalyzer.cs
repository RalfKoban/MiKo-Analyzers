using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    //// TODO: Potential NRE in code? --> AD0001 reports that

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

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation) => symbol.GetMethods(MethodKind.Ordinary).Select(AnalyzeOrdinaryMethod).Where(_ => _ != null);

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

            if (ForbiddenTypes.Any(returnType.ImplementsPotentialGeneric))
            {
                // detect for same assembly to avoid AD0001 (which reports that the return type is in a different compilation than the method/property)
                var sameAssembly = method.ContainingAssembly.Equals(returnType.ContainingAssembly, SymbolEqualityComparer.IncludeNullability);

                if (returnType.Locations.IsEmpty || sameAssembly is false)
                {
                    var syntax = (MethodDeclarationSyntax)method.GetSyntax();

                    return Issue(returnTypeString, syntax.ReturnType);
                }

                return Issue(returnType);
            }

            return null;
        }
    }
}