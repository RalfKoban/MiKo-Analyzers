using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1098_TypeNameFollowsInterfaceNameSchemeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1098";

        public MiKo_1098_TypeNameFollowsInterfaceNameSchemeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Struct:
                    return AnalyzeName(symbol);

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        private static List<string> CollectNames(ImmutableArray<INamedTypeSymbol> directlyImplementedInterfaces)
        {
            var names = new List<string>();

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var length = directlyImplementedInterfaces.Length;

            for (var index = 0; index < length; index++)
            {
                var implementedInterface = directlyImplementedInterfaces[index];
                var name = implementedInterface.Name
                                               .WithoutNumberSuffix() // ignore number suffixes as they normally indicate only another version of the specific interface
                                               .Without("Internal"); // ignore internal markers

                if (name.EndsWith("able", StringComparison.Ordinal) && name.EndsWith("Table", StringComparison.Ordinal) is false)
                {
                    // ignore ability interfaces
                    continue;
                }

                if (name.StartsWith("IHas", StringComparison.Ordinal) && name.Length > 5 && name[4].IsUpperCaseLetter())
                {
                    // ignore IHas ability interfaces
                    continue;
                }

                if (name.EndsWith("Notification", StringComparison.Ordinal))
                {
                    // ignore callback interfaces
                    continue;
                }

                if (name.EndsWith("Provider", StringComparison.Ordinal))
                {
                    // ignore providers, such as ToolTipProvider or ContentProvider
                    continue;
                }

                if (name.Contains("Extended", StringComparison.Ordinal))
                {
                    // remove extended name as that has no additional business value
                    name = name.Without("Extended");
                }

                if (name.EndsWith("Command", StringComparison.Ordinal))
                {
                    // commands should be suffixed with 'Command' only
                    name = "Command";
                }

                switch (name)
                {
                    case nameof(IChangeTracking):
                    case nameof(IDeserializationCallback):
                    case nameof(INotifyCollectionChanged):
                    case nameof(INotifyDataErrorInfo):
                    case nameof(INotifyPropertyChanged):
                    case nameof(INotifyPropertyChanging):
                        break; // ignore those specific interfaces

                    case Constants.Names.IValueConverter:
                    case Constants.Names.IMultiValueConverter:
                    {
                        // should be named converter at least
                        names.Add("Converter");

                        break;
                    }

                    default:
                        names.Add(name.StartsWith('I') ? name.Substring(1) : name);

                        break;
                }
            }

            return names;
        }

        private IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var directlyImplementedInterfaces = symbol.Interfaces;

            if (directlyImplementedInterfaces.Any())
            {
                var names = CollectNames(directlyImplementedInterfaces);

                if (names.Any() && symbol.Name.EndsWithAny(names, StringComparison.Ordinal) is false)
                {
                    return new[] { Issue(symbol, names.HumanizedConcatenated()) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}