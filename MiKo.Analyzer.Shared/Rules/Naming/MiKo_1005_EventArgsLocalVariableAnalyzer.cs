using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1005_EventArgsLocalVariableAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1005";

        private const string E = "e";
        private const string Args = "args";

        public MiKo_1005_EventArgsLocalVariableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsEventArgs() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            var identifiersLength = identifiers.Length;

            for (var index = 0; index < identifiersLength; index++)
            {
                var identifier = identifiers[index];
                var name = identifier.ValueText;

                switch (name)
                {
                    case E:
                    case Args:
                        continue;

                    default:
                    {
                        var symbol = identifier.GetSymbol(semanticModel);

                        // there might be methods that have a parameter named 'e', thus we have to use 'args' instead
                        var method = identifier.Parent.GetEnclosingMethod(semanticModel);
                        var proposal = GetProposedName(method);

                        yield return Issue(symbol, proposal, CreateBetterNameProposal(proposal));

                        break;
                    }
                }
            }
        }

        private static string GetProposedName(IMethodSymbol method)
        {
            var parameters = method.Parameters;

            return parameters.Length > 0 && parameters.Any(_ => _.Name == E) ? Args : E;
        }
    }
}