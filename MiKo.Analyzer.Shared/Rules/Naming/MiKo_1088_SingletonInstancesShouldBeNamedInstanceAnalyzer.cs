using System;
using System.Collections.Generic;
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
        private const string AllowedName1 = "Empty";
        private const string AllowedName2 = "Default";

        private static readonly HashSet<string> FieldNames = Enumerable.Empty<string>()
                                                                       .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + InstanceName.ToLowerCaseAt(0)))
                                                                       .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + InstanceName))
                                                                       .ToHashSet();

        private static readonly HashSet<string> AllowedFieldNames = Enumerable.Empty<string>()
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName1.ToLowerCaseAt(0)))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName1))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName2.ToLowerCaseAt(0)))
                                                                              .Concat(Constants.Markers.FieldPrefixes.Select(_ => _ + AllowedName2))
                                                                              .ToHashSet();

        public MiKo_1088_SingletonInstancesShouldBeNamedInstanceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsEnum() || symbol.IsTestClass())
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
                switch (property.Name)
                {
                    case InstanceName:
                    case AllowedName1:
                    case AllowedName2:
                    {
                        continue;
                    }

                    default:
                    {
                        yield return Issue(property);

                        break;
                    }
                }
            }

            foreach (var field in selfReferencingFields)
            {
                if (FieldNames.Contains(field.Name) is false && AllowedFieldNames.Contains(field.Name) is false)
                {
                    yield return Issue(field);
                }
            }
        }
    }
}