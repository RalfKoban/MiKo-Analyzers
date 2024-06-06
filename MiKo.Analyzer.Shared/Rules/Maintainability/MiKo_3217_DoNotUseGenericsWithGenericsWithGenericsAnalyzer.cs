using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private static bool HasIssue(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol type)
            {
                switch (symbol.Name)
                {
                    case Constants.Moq.Mock:
                        return false; // ignore mocks completely

                    case "Action":
                    case "Func":
                    case "Expression":
                    case "Predicate":
                    case nameof(Task):
                    {
                        var arguments = type.TypeArguments;

                        return arguments.Length > 0 && arguments.Any(HasGenericTypeArgument);
                    }
                }
            }

            return HasGenericTypeArgument(symbol);
        }

        private static bool HasGenericTypeArgument(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol type)
            {
                var arguments = type.TypeArguments;

                return arguments.Length > 0 && arguments.OfType<INamedTypeSymbol>().Any(_ => _.IsGeneric());
            }

            return false;
        }
    }
}