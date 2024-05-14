using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1088_SingletonInstancesShouldBeNamedInstanceAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1088";

        private const string InstanceName = "Instance";

        private const string AllowedName1 = "Default";
        private const string AllowedName2 = "Empty";
        private const string AllowedName3 = "Zero";

        private static readonly HashSet<string> AllowedFieldNames = Enumerable.Empty<string>()
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + InstanceName.ToLowerCaseAt(0)))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + InstanceName))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName1.ToLowerCaseAt(0)))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName1))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName2.ToLowerCaseAt(0)))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName2))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName3.ToLowerCaseAt(0)))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName3))
                                                                              .ToHashSet();

        private static readonly HashSet<string> AllowedPropertyNames = new HashSet<string> { InstanceName, AllowedName1, AllowedName2, AllowedName3 };

        public MiKo_1088_SingletonInstancesShouldBeNamedInstanceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.TypeKind != TypeKind.Class || symbol.IsRecord || symbol.IsTestClass())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return Analyze(symbol);
        }

        private IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol)
        {
            var selfReferencingProperties = symbol.GetProperties().Where(_ => _.IsStatic && symbol.Equals(_.GetReturnType(), SymbolEqualityComparer.Default));
            var selfReferencingFields = symbol.GetFields().Where(_ => _.IsStatic && symbol.Equals(_.Type, SymbolEqualityComparer.Default));

            foreach (var property in selfReferencingProperties)
            {
                if (AllowedPropertyNames.Contains(property.Name) is false)
                {
                    yield return Issue(property);
                }
            }

            foreach (var field in selfReferencingFields)
            {
                if (AllowedFieldNames.Contains(field.Name) is false)
                {
                    yield return Issue(field);
                }
            }
        }
    }
}