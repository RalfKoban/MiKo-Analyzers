using System;
using System.Collections.Generic;

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

        internal static string FindBetterName(ISymbol symbol) => GetPluralName(symbol.Name, out _);

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

                var pluralName = GetPluralName(originalName, out var name);
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
                case "map":
                case "list":
                case "array":
                case "stack":
                case "queue":
                case "buffer":
                case "result":
                {
                    return true;
                }

                default:
                    return false;
            }
        }

        private static string GetPluralName(string originalName, out string name)
        {
            name = originalName.EndsWithNumber() ? originalName.WithoutNumberSuffix() : originalName;

            if (name.EndsWithAny(Constants.Markers.Collections))
            {
                return Pluralizer.GetPluralName(name, StringComparison.OrdinalIgnoreCase, Constants.Markers.Collections);
            }

            return Pluralizer.GetPluralName(name);
        }
    }
}