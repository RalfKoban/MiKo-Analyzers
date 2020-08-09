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

        internal static string FindBetterName(ISymbol symbol)
        {
            var method = symbol.GetEnclosingMethod();

            return GetProposedName(method);
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsEventArgs() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;
                if (name == E || name == Args)
                {
                    continue;
                }

                var symbol = identifier.GetSymbol(semanticModel);

                // there might be methods that have a parameter named 'e', thus we have to use 'args' instead
                var method = identifier.Parent.GetEnclosingMethod(semanticModel);
                var proposedName = GetProposedName(method);

                yield return Issue(symbol, proposedName);
            }
        }

        private static string GetProposedName(IMethodSymbol method) => method.Parameters.Any(_ => _.Name == E) ? Args : E;
    }
}