using System;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1037_TypeSuffixAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1037";

        private static readonly string[] WrongSuffixes =
                                                         {
                                                             // order is important here because of the order during removal
                                                             "Enums", "Enum",
                                                             "Types", "Type",
                                                             "Interfaces", "Interface",
                                                             "Classes", "Class",
                                                             "Structs", "Struct",
                                                             "Records", "Record",
                                                         };

        private static readonly string[] AllowedEnumSuffixes = { "Interfaces", "Interface", "Classes", "Class" };

        public MiKo_1037_TypeSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(string typeName, BaseTypeDeclarationSyntax declaration)
        {
            if (declaration is EnumDeclarationSyntax && typeName.EndsWithAny(AllowedEnumSuffixes))
            {
                return false;
            }

            return typeName.EndsWithAny(WrongSuffixes, StringComparison.OrdinalIgnoreCase);
        }

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            var betterName = FindBetterName(typeName, declaration);

            if (betterName.IsNullOrWhiteSpace())
            {
                return Array.Empty<Diagnostic>();
            }

            return new[] { Issue(typeNameIdentifier, betterName) };
        }

        private static string FindBetterName(string typeName, BaseTypeDeclarationSyntax declaration)
        {
            var betterName = typeName.AsCachedBuilder()
                                     .ReplaceWithProbe("TypeEnum", "Kind")
                                     .Without(WrongSuffixes)
                                     .ToStringAndRelease();

            if (betterName.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            if (declaration is EnumDeclarationSyntax && betterName.EndsWith('s') is false && declaration.HasAttributeName(Constants.Names.FlagsAttributeNames))
            {
                betterName = Pluralizer.GetPluralName(typeName, betterName);
            }

            return betterName;
        }
    }
}