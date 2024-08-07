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

        private static readonly string[] Splitters = { "Of", "With", "To", "In", "From" };

        public MiKo_1070_CollectionLocalVariableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol)
        {
            if (symbol.Name == "AssemblyCatalog")
            {
                // ignore MEF aggregate catalog
                return false;
            }

            return symbol.IsEnumerable() && IsXmlNode(symbol) is false;
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

                if (originalName.EndsWith("Map", StringComparison.Ordinal))
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
                    yield return Issue(originalName, identifier, pluralName, CreateBetterNameProposal(pluralName));
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
                case Constants.LambdaIdentifiers.Fallback0:
                case Constants.LambdaIdentifiers.Fallback1:
                case Constants.LambdaIdentifiers.Fallback2:
                case Constants.LambdaIdentifiers.Fallback3:
                case Constants.LambdaIdentifiers.Fallback4:
                case Constants.LambdaIdentifiers.Fallback5:
                case Constants.LambdaIdentifiers.Fallback2Underscores:
                case Constants.LambdaIdentifiers.Fallback3Underscores:
                case Constants.LambdaIdentifiers.Fallback4Underscores:
                case "map":
                case "set":
                case "list":
                case "array":
                case "stack":
                case "queue":
                case "buffer":
                case "result":
                case "mapping":
                {
                    return true;
                }

                default:
                    return false;
            }
        }

        private static bool IsHash(string name, ITypeSymbol type) => name.EndsWith("Hash", StringComparison.OrdinalIgnoreCase) && type?.IsByteArray() is true;

        private static bool IsGrouping(string name, ITypeSymbol type)
        {
            switch (name)
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

        private static string GetPluralName(string originalName, out string name)
        {
            if (originalName.EndsWith('s'))
            {
                name = originalName;

                return originalName;
            }

            var index = originalName.IndexOfAny(Splitters, StringComparison.Ordinal);

            if (index > 0)
            {
                var nameToInspect = originalName.Substring(0, index);
                var remainingPart = originalName.Substring(index);

                var pluralName = GetPluralName(nameToInspect, out name);

                name = string.Concat(name, remainingPart);

                return pluralName + remainingPart;
            }

            name = originalName.EndsWithNumber() ? originalName.WithoutNumberSuffix() : originalName;

            if (name.EndsWithAny(Constants.Markers.Collections))
            {
                return Pluralizer.GetPluralName(name, StringComparison.OrdinalIgnoreCase, Constants.Markers.Collections);
            }

            return Pluralizer.GetPluralName(name);
        }
    }
}