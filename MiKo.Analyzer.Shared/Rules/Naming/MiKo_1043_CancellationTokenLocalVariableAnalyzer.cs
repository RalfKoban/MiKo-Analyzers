using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1043_CancellationTokenLocalVariableAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1043";

        private const string ExpectedName = "token";

        public MiKo_1043_CancellationTokenLocalVariableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol?.IsCancellationToken() is true;

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            var length = identifiers.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var identifier = identifiers[index];

                    if (identifier.ValueText != ExpectedName)
                    {
                        var symbol = identifier.GetSymbol(semanticModel);

                        yield return Issue(symbol, ExpectedName, CreateBetterNameProposal(ExpectedName));
                    }
                }
            }
        }
    }
}