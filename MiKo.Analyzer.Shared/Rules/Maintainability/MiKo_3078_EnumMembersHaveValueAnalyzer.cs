using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3078_EnumMembersHaveValueAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3078";

        public MiKo_3078_EnumMembersHaveValueAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEnum();

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            var fields = symbol.GetFields().ToList();

            var isFlagged = symbol.HasAttribute(Constants.Names.FlagsAttributeNames);

            foreach (var field in fields)
            {
                var syntax = field.GetSyntax<EnumMemberDeclarationSyntax>();

                if (syntax.EqualsValue is null)
                {
                    yield return Issue(
                                   field,
                                   new Pair(Constants.AnalyzerCodeFixSharedData.Position, fields.IndexOf(field).ToString("D")),
                                   new Pair(Constants.AnalyzerCodeFixSharedData.IsFlagged, isFlagged.ToString()));
                }
            }
        }
    }
}