using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                typeof(ICollection)
            };

        public MiKo_3008_ListReturnValueAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Interface
                                                                                               ? symbol.GetMembers().OfType<IMethodSymbol>().Where(_ => _.MethodKind == MethodKind.Ordinary).SelectMany(AnalyzeMethod)
                                                                                               : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.ReturnsVoid) return Enumerable.Empty<Diagnostic>();

            var returnType = method.ReturnType;

            switch (returnType.SpecialType)
            {
                case SpecialType.System_Array:
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_ICollection_T:
                    return new[] { ReportIssue(method) };

                case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    return Enumerable.Empty<Diagnostic>();
            }

            if (returnType.ToString() == "byte[]") return Enumerable.Empty<Diagnostic>();

            return ForbiddenTypes.Any(returnType.ImplementsPotentialGeneric)
                       ? new[] { ReportIssue(returnType) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}