using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1050_ReturnValueLocalVariableAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1050";

        private static readonly HashSet<string> AllowedCompleteNames = new HashSet<string>
                                                                           {
                                                                               "result",
                                                                               "results",
                                                                           };

        private static readonly string[] WrongNames = CreateWrongNames(AllowedCompleteNames, "ret", "return", "returned", "returning", "retval", "res", "resulting");

        public MiKo_1050_ReturnValueLocalVariableAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName() => "result";

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => AnalyzeIdentifiers(semanticModel, identifiers);

        private static string[] CreateWrongNames(IEnumerable<string> values, params string[] additionalValues)
        {
            var results = new HashSet<string>();

            foreach (var value in values.Concat(additionalValues))
            {
                results.Add(value);
                results.Add(value + "_");
            }

            return results.ToArray();
        }

        private static bool ContainsWrongName(string identifier, IEnumerable<string> wrongNames)
        {
            var found = false;

            foreach (var wrongName in wrongNames)
            {
                if (identifier.Length < wrongName.Length)
                {
                    continue;
                }

                if (identifier.Length == wrongName.Length)
                {
                    if (identifier.Equals(wrongName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (AllowedCompleteNames.Contains(identifier))
                        {
                            continue;
                        }

                        return true;
                    }
                }
                else
                {
                    var nextCharacter = identifier[wrongName.Length];

                    if (nextCharacter.IsLowerCaseLetter())
                    {
                        continue;
                    }

                    if (identifier.StartsWith(wrongName, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                    }
                }
            }

            return found;
        }

        private IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, IEnumerable<SyntaxToken> identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var name = identifier.ValueText;

                var isWrong = ContainsWrongName(name, WrongNames);

                if (isWrong)
                {
                    var symbol = identifier.GetSymbol(semanticModel);

                    yield return Issue(symbol, name);
                }
            }
        }
    }
}