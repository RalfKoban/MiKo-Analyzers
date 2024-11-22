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
            var identifierNames = fieldSyntax.DescendantNodes<IdentifierNameSyntax>().ToList();

            if (identifierNames.Count != 0)
            {
                var fieldLocation = fieldSyntax.GetLocation().GetLineSpan();

                // get all fields
                var problematicFields = GetStaticFieldsFromBelowOrFromOtherPart(symbol, fieldLocation);
                var problematicFieldNames = problematicFields.SelectMany(_ => _.Declaration.Variables).ToHashSet(_ => _.GetName());

                List<string> wrongReferences = null;

                foreach (var identifier in identifierNames)
                {
                    var name = identifier.GetName();

                    if (problematicFieldNames.Contains(name) && identifier.GetSymbol(compilation) is IFieldSymbol f && f.ContainingType.Equals(symbol.ContainingType, SymbolEqualityComparer.Default))
                    {
                        if (wrongReferences is null)
                        {
                            wrongReferences = new List<string>();
                        }

                        wrongReferences.Add(name);
                    }
                }

                if (wrongReferences != null)
                {
                    return new[] { Issue(symbol, wrongReferences.HumanizedConcatenated("and")) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
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

                // we might not find the other field's syntax node, so be prepared for null
                var location = otherFieldSyntax?.GetLocation();

                if (location?.IsInSource is true)
                {
                    yield return otherFieldSyntax;
                }
            }
        }
    }
}