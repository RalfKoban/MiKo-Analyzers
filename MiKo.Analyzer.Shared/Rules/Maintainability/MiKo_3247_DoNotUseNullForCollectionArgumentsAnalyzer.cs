using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3247";

        public MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNull, SyntaxKind.NullLiteralExpression);

        private static bool HasIssue(LiteralExpressionSyntax literal, in SyntaxNodeAnalysisContext context)
        {
            if (literal.Parent is ArgumentSyntax argument && argument.Parent is ArgumentListSyntax list && list.Parent is InvocationExpressionSyntax invocation)
            {
                if (invocation.GetSymbol(context.SemanticModel) is IMethodSymbol method)
                {
                    var arguments = list.Arguments;
                    var parameters = method.Parameters;
                    var parameter = FindParameter(argument, arguments, parameters);

                    if (parameter is null)
                    {
                        // seems like an additional parameter of a params array, so ignore that
                        return false;
                    }

                    if (parameter.IsParams)
                    {
                        // ignore when we have multiple parameters for the params array, but report if null is the only parameter
                        return arguments.Count <= parameters.Length;
                    }

                    if (parameter.Type.IsEnumerable())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static IParameterSymbol FindParameter(ArgumentSyntax argument, in SeparatedSyntaxList<ArgumentSyntax> arguments, in ImmutableArray<IParameterSymbol> parameters)
        {
            if (argument.NameColon is NameColonSyntax named)
            {
                var argumentName = named.GetName();

                return parameters.First(_ => _.Name == argumentName);
            }

            var index = arguments.IndexOf(argument);

            return index < parameters.Length
                   ? parameters[index]
                   : null;
        }

        private void AnalyzeNull(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LiteralExpressionSyntax literal && HasIssue(literal, context))
            {
                ReportDiagnostics(context, Issue(literal));
            }
        }
    }
}