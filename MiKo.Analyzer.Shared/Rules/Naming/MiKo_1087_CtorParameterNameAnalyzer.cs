using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1087_CtorParameterNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1087";

        public MiKo_1087_CtorParameterNameAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var baseType = symbol.ContainingType.BaseType;

            if (baseType is null)
            {
                yield break;
            }

            var syntax = symbol.GetSyntax<ConstructorDeclarationSyntax>();

            if (syntax is null)
            {
                // may happen if we have a primary constructor on a record
                yield break;
            }

            var baseCall = syntax.FirstChild<ConstructorInitializerSyntax>(SyntaxKind.BaseConstructorInitializer);

            if (baseCall is null)
            {
                yield break;
            }

            var arguments = baseCall.ArgumentList.Arguments;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var argumentsCount = arguments.Count;

            if (argumentsCount == 0)
            {
                // no arguments to check for
                yield break;
            }

            var mapping = new Dictionary<string, int>();

            for (var i = 0; i < argumentsCount; i++)
            {
                var argument = arguments[i];

                if (argument.Expression is IdentifierNameSyntax identifier)
                {
                    // if we have multiple arguments using the same value, then we overwrite them to only use the last one
                    mapping[identifier.GetName()] = i;
                }
            }

            // symbol parameters
            if (baseCall.GetSymbol(compilation) is IMethodSymbol baseCtor)
            {
                var parameters = symbol.Parameters;
                var baseCtorParameters = baseCtor.Parameters;

                // check for each parameter whether the base parameter has the same name as any of the own parameters or whether it has a different one
                foreach (var parameter in parameters)
                {
                    var parameterName = parameter.Name;

                    if (mapping.TryGetValue(parameterName, out var argumentPosition))
                    {
                        // we found a parameter, hence we check the name on the base ctor
                        if (argumentPosition >= baseCtorParameters.Length)
                        {
                            // seems to not match because we have some uncompilable code
                            continue;
                        }

                        var baseParameter = baseCtorParameters[argumentPosition];
                        var baseParameterName = baseParameter.Name;

                        if (baseParameterName != parameterName)
                        {
                            yield return Issue(parameter, baseParameterName, CreateBetterNameProposal(baseParameterName));
                        }
                    }
                }
            }
        }
    }
}