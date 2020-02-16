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

        public MiKo_1005_EventArgsLocalVariableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsEventArgs() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            const string E = "e";
            const string Args = "args";

            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;

                if (name != E && name != Args)
                {
                    var symbol = identifier.GetSymbol(semanticModel);

                    // there might be methods that have a parameter named 'e', thus we have to use 'args' instead
                    var method = identifier.Parent.GetEnclosingMethod(semanticModel);
                    var proposedName = method.Parameters.Any(_ => _.Name == E) ? Args : E;

                    yield return Issue(symbol, proposedName);
                }
            }
        }
    }
}