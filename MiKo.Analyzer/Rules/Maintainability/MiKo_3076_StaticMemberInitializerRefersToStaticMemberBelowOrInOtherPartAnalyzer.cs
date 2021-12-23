using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3076_StaticMemberInitializerRefersToStaticMemberBelowOrInOtherPartAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3076";

        public MiKo_3076_StaticMemberInitializerRefersToStaticMemberBelowOrInOtherPartAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => IsStaticField(symbol);

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation)
        {
            var fieldSyntax = symbol.GetSyntax<FieldDeclarationSyntax>();
            var identifierNames = fieldSyntax.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();

            if (identifierNames.None())
            {
                // nothing to inspect
                return Enumerable.Empty<Diagnostic>();
            }

            var fieldLocation = fieldSyntax.GetLocation().GetLineSpan();

            // get all fields
            var problematicFields = GetStaticFieldsFromBelowOrFromOtherPart(symbol, fieldLocation);
            var problematicFieldNames = problematicFields.SelectMany(_ => _.Declaration.Variables).Select(_ => _.GetName()).ToHashSet();

            var wrongReferences = new List<string>();
            foreach (var identifier in identifierNames)
            {
                var name = identifier.GetName();

                if (problematicFieldNames.Contains(name) && identifier.GetSymbol(compilation) is IFieldSymbol f && f.ContainingType == symbol.ContainingType)
                {
                    wrongReferences.Add(name);
                }
            }

            return wrongReferences.Any()
                       ? new[] { Issue(symbol, wrongReferences.HumanizedConcatenated("and")) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static bool IsStaticField(IFieldSymbol symbol) => symbol.IsStatic && symbol.IsConst is false;

        private static IEnumerable<FieldDeclarationSyntax> GetStaticFieldsFromBelowOrFromOtherPart(IFieldSymbol symbol, FileLinePositionSpan fieldLocation)
        {
            var otherFields = GetOtherStaticFields(symbol);

            foreach (var otherField in otherFields)
            {
                var lineSpan = otherField.GetLocation().GetLineSpan();

                if (lineSpan.Path != fieldLocation.Path || lineSpan.StartLinePosition >= fieldLocation.StartLinePosition)
                {
                    yield return otherField;
                }
            }
        }

        private static IEnumerable<FieldDeclarationSyntax> GetOtherStaticFields(IFieldSymbol field)
        {
            var otherStaticFields = field.ContainingType.GetFields().Where(IsStaticField).ToList();
            otherStaticFields.Remove(field);

            foreach (var otherField in otherStaticFields)
            {
                var otherFieldSyntax = otherField.GetSyntax<FieldDeclarationSyntax>();
                var location = otherFieldSyntax.GetLocation();

                if (location.IsInSource)
                {
                    yield return otherFieldSyntax;
                }
            }
        }
    }
}