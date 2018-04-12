using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
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
                                                                                               ? symbol.GetMembers().OfType<IMethodSymbol>().Select(AnalyzeOrdinaryMethod).Where(_ => _ != null).ToList()
                                                                                               : Enumerable.Empty<Diagnostic>();

        private Diagnostic AnalyzeOrdinaryMethod(IMethodSymbol method)
        {
            if (method.MethodKind != MethodKind.Ordinary) return null;
            if (method.ReturnsVoid) return null;

            var returnType = method.ReturnType;

            switch (returnType.SpecialType)
            {
                case SpecialType.System_Array:
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_ICollection_T:
                    return ReportIssue(method);

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
                return null;

            if (ForbiddenTypes.Any(returnType.ImplementsPotentialGeneric))
            {
                if (returnType.Locations.IsEmpty)
                {
                    var syntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences[0].GetSyntax();
                    return ReportIssue(returnTypeString, syntax.ReturnType.GetLocation());
                }

                return ReportIssue(returnType);
            }

            return null;
        }
    }
}