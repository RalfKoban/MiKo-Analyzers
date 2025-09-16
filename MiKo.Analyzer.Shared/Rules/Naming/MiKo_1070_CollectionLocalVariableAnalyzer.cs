using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1070_CollectionLocalVariableAnalyzer : LocalVariableNamingAnalyzer
    {
        public const string Id = "MiKo_1070";

        private static readonly string[] TestRelatedNames = { "actual", "expected" };

        public MiKo_1070_CollectionLocalVariableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            if (IsMefAggregateCatalog(symbolName))
            {
                // ignore MEF aggregate catalog
                return false;
            }

            if (symbol.IsEnumerable())
            {
                if (IsDocument(symbolName))
                {
                    return false;
                }

                if (symbol.IsXmlNode())
                {
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

                if (IsShort(originalName))
                {
                    // skip all short names
                    continue;
                }

                if (originalName.EndsWith("Map", StringComparison.Ordinal))
                {
                    continue;
                }

                if (originalName.EqualsAny(TestRelatedNames))
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

                var name = originalName;
                var span = originalName.AsSpan();
                var pluralName = span.EndsWith('s')
                                 ? originalName
                                 : FindPluralName(span, out name);  // might return null in case there is none

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
                case Constants.LambdaIdentifiers.FallbackUnderscores2:
                case Constants.LambdaIdentifiers.FallbackUnderscores3:
                case Constants.LambdaIdentifiers.FallbackUnderscores4:
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDocument(string typeName) => typeName.EndsWith("Document", StringComparison.Ordinal);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMefAggregateCatalog(string typeName) => typeName is "AssemblyCatalog";
    }
}