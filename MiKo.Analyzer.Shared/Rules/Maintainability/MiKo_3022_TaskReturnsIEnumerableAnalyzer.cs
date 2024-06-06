using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3022_TaskReturnsIEnumerableAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3022";

        public MiKo_3022_TaskReturnsIEnumerableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.ReturnType.IsTask() && symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var typeArguments = ((INamedTypeSymbol)symbol.ReturnType).TypeArguments;

            if (typeArguments.Length == 1 && typeArguments[0] is INamedTypeSymbol typeArgument)
            {
                var specialType = typeArgument.SpecialType;

                switch (specialType)
                {
                    case SpecialType.System_Collections_IEnumerable:
                    case SpecialType.System_Collections_Generic_IEnumerable_T:
                    {
                        return new[] { ReportIssue(symbol) };
                    }
                }

                if (typeArgument.TypeKind == TypeKind.Interface && typeArgument.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                {
                    return new[] { ReportIssue(symbol) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private Diagnostic ReportIssue(IMethodSymbol method)
        {
            var returnTypeName = method.ReturnType.MinimalTypeName();

            return method.GetSyntax() is MethodDeclarationSyntax m
                   ? Issue(m.GetName(), m.ReturnType, returnTypeName)
                   : Issue(method, returnTypeName);
        }
    }
}