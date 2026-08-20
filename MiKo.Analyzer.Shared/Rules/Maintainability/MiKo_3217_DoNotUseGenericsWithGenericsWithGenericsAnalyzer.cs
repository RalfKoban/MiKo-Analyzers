using System;
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
            List<Diagnostic> issues = null;

            if (symbol.ReturnsVoid is false)
            {
                var returnType = symbol.ReturnType;

                if (HasIssue(returnType))
                {
                    issues = new List<Diagnostic>(1);

                    issues.Add(IssueOnType(returnType, symbol));
                }
            }

            var parameters = symbol.Parameters;
            var parametersLength = parameters.Length;

            if (parametersLength > 0)
            {
                for (var index = 0; index < parametersLength; index++)
                {
                    var parameter = parameters[index];
                    var parameterType = parameter.Type;

                    if (HasIssue(parameterType))
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(IssueOnType(parameterType, parameter));
                    }
                }
            }

            return (IEnumerable<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation)
        {
            var type = symbol.Type;

            return HasIssue(type)
                   ? new[] { IssueOnType(type, symbol) }
                   : Array.Empty<Diagnostic>();
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
                    case nameof(ValueTuple):
                    {
                        var arguments = type.TypeArguments;

                        return arguments.Length > 0 && arguments.Any(HasNestedGenericTypeArguments);
                    }
                }
            }

            return HasNestedGenericTypeArguments(symbol);
        }

        private static bool HasNestedGenericTypeArguments(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol type)
            {
                var arguments = type.TypeArguments;

                if (arguments.Length > 0)
                {
                    return arguments.SkipWhere(_ => _.IsNullable())
                                    .Any(_ => _.HasGenericTypeArguments());
                }
            }

            return false;
        }
    }
}