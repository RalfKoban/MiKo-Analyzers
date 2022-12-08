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

        private const string  BetterName = "BetterName";

        public MiKo_1087_CtorParameterNameAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IParameterSymbol symbol, Diagnostic diagnostic) => diagnostic.Properties[BetterName];

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsConstructor();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var baseType = symbol.ContainingType.BaseType;
            if (baseType is null)
            {
                yield break;
            }

            var syntax = symbol.GetSyntax<ConstructorDeclarationSyntax>();
            var baseCall = syntax.FirstChild<ConstructorInitializerSyntax>(SyntaxKind.BaseConstructorInitializer);
            if (baseCall is null)
            {
                yield break;
            }

            var mapping = new Dictionary<string, int>();

            var arguments = baseCall.ArgumentList.Arguments;
            for (var i = 0; i < arguments.Count; i++)
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
                foreach (var parameter in symbol.Parameters)
                {
                    var parameterName = parameter.Name;

                    if (mapping.TryGetValue(parameterName, out var argumentPosition))
                    {
                        // we found a parameter, hence we check the name on the base ctor
                        var baseParameter = baseCtor.Parameters[argumentPosition];
                        var baseParameterName = baseParameter.Name;

                        if (baseParameterName != parameterName)
                        {
                            yield return Issue(parameter, baseParameterName, new Dictionary<string, string> { { BetterName, baseParameterName } });
                        }
                    }
                }

                // check for each parameter whether the base parameter has the same name as any of the own parameters or whether it has a different one
            }
        }
    }
}