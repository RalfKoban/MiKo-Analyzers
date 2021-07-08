using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1009_EventHandlerLocalVariableAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1009";

        private const string Handler = "handler";

        public MiKo_1009_EventHandlerLocalVariableAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(ISymbol symbol) => Handler;

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsEventHandler() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;
                if (name != Handler)
                {
                    var symbol = identifier.GetSymbol(semanticModel);
                    yield return Issue(symbol, Handler);
                }
            }
        }
    }
}