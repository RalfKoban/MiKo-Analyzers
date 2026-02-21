using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4105_ObjectUnderTestFieldOrderingAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4105";

        public MiKo_4105_ObjectUnderTestFieldOrderingAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var field = symbol.GetFields().FirstOrDefault(_ => _.IsConst is false && Constants.Names.TypeUnderTestFieldNames.Contains(_.Name));

            return field is null
                   ? Array.Empty<Diagnostic>()
                   : AnalyzeTestType(symbol, field);
        }

        private static IFieldSymbol GetFirstRemainingField(IFieldSymbol field, IEnumerable<IFieldSymbol> fields)
        {
            IEnumerable<IFieldSymbol> remainingFields = fields.SkipWhile(_ => _.IsConst);

            if (field.IsStatic is false)
            {
                remainingFields = remainingFields.SkipWhile(_ => _.IsStatic);
            }

            if (field.IsReadOnly is false)
            {
                remainingFields = remainingFields.SkipWhile(_ => _.IsReadOnly);
            }

            return remainingFields.FirstOrDefault();
        }

        private Diagnostic[] AnalyzeTestType(INamedTypeSymbol symbol, IFieldSymbol field)
        {
            var path = field.GetLineSpan().Path;

            var fields = GetFieldsOrderedByLocation(symbol, path);
            var firstField = GetFirstRemainingField(field, fields);

            return ReferenceEquals(field, firstField)
                   ? Array.Empty<Diagnostic>()
                   : new[] { Issue(field) };
        }
    }
}