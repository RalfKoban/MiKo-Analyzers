using System;
using System.Collections.Generic;
using System.Xml;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

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

        protected override bool ShallAnalyze(ITypeSymbol symbol)
        {
            if (symbol.IsEnumerable())
            {
                if (IsXmlNode(symbol))
                {
                    return false;
                }

                if (symbol.Name == "AssemblyCatalog")
                {
                    // ignore MEF aggregate catalog
                    return false;
                }

                return true;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers)
        {
            foreach (var identifier in identifiers)
            {
                var originalName = identifier.ValueText;

                // skip all short names
                if (IsShort(originalName))
                {
                    continue;
                }

                if (IsHash(originalName, type))
                {
                    continue;
                }

                if (IsGrouping(originalName, type))
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
                case Constants.LambdaIdentifiers.Fallback3:
                case "map":
                case "set":
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

        private static bool IsHash(string originalName, ITypeSymbol type)
        {
            if (originalName.EndsWith("Hash", StringComparison.OrdinalIgnoreCase))
            {
                return type?.IsByteArray() is true;
            }

            return false;
        }

        private static bool IsGrouping(string originalName, ITypeSymbol type)
        {
            switch (originalName)
            {
                case "@group":
                case "group":
                case "grouping":
                {
                    return type?.IsIGrouping() is true;
                }
            }

            return false;
        }

        private static bool IsXmlNode(ITypeSymbol type)
        {
            switch (type?.Name)
            {
                case nameof(XmlDocument):
                case nameof(XmlElement):
                case nameof(XmlNode):
                {
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}