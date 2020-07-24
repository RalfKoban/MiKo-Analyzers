using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1070_CollectionLocalVariableAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1070";

        public MiKo_1070_CollectionLocalVariableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsEnumerable() && symbol.Name != "AssemblyCatalog"; // ignore MEF aggregate catalog

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var originalName = identifier.ValueText;

                // skip all short names
                if (IsShort(originalName))
                {
                    continue;
                }

                var name = originalName.EndsWithNumber() ? originalName.WithoutNumberSuffix() : originalName;

                var pluralName = name.EndsWithAny(Constants.Markers.Collections)
                                     ? GetPluralName(name, StringComparison.OrdinalIgnoreCase, Constants.Markers.Collections)
                                     : GetPluralName(name);

                if (pluralName is null)
                {
                    continue;
                }

                if (pluralName != name)
                {
                    yield return Issue(originalName, identifier, pluralName);
                }
            }
        }

        private static bool IsShort(string name)
        {
            if (name.Length <= 1)
            {
                return true;
            }

            switch (name)
            {
                case Constants.LambdaIdentifiers.Default:
                case Constants.LambdaIdentifiers.Fallback:
                case Constants.LambdaIdentifiers.Fallback2:
                case "list":
                case "array":
                case "buffer":
                case "result":
                case "stack":
                case "queue":
                {
                    return true;
                }

                default:
                    return false;
            }
        }
    }
}