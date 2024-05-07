using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3217_DoNotUseGenericsWithGenericsWithGenericsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3217";

        public MiKo_3217_DoNotUseGenericsWithGenericsWithGenericsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (base.ShallAnalyze(symbol))
            {
                switch (symbol.MethodKind)
                {
                    case MethodKind.PropertyGet:
                    case MethodKind.PropertySet:
                        return false;

                    default:
                        return symbol.IsInterfaceImplementation() is false;
                }
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.ReturnsVoid is false)
            {
                if (HasIssue(symbol.ReturnType))
                {
                    yield return IssueOnType(symbol.ReturnType, symbol);
                }
            }

            foreach (var parameter in symbol.Parameters)
            {
                if (HasIssue(parameter.Type))
                {
                    yield return IssueOnType(parameter.Type, parameter);
                }
            }
        }

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation)
        {
            if (HasIssue(symbol.Type))
            {
                yield return IssueOnType(symbol.Type, symbol);
            }
        }

        private static bool HasIssue(ITypeSymbol returnType) => returnType is INamedTypeSymbol type && returnType.IsTask()
                                                                ? type.TypeArguments.Any(HasGenericTypeArgument)
                                                                : HasGenericTypeArgument(returnType);

        private static bool HasGenericTypeArgument(ITypeSymbol symbol) => symbol is INamedTypeSymbol type
                                                                       && type.TypeArguments.OfType<INamedTypeSymbol>().Any(_ => _.IsGeneric());
    }
}