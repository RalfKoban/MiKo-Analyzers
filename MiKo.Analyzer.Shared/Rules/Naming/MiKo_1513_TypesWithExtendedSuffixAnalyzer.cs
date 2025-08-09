using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1513_TypesWithExtendedSuffixAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1513";

        private static readonly string[] WrongSuffixes = { "Advanced", "Complex", "Enhanced", "Extended", "Simple", "Simplified" };

        public MiKo_1513_TypesWithExtendedSuffixAnalyzer() : base(Id)
        {
        }

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            foreach (var suffix in WrongSuffixes)
            {
                var symbolName = typeName.AsSpan();

                if (symbolName.EndsWith(suffix))
                {
                    var proposal = suffix.ConcatenatedWith(symbolName.WithoutSuffix(suffix));

                    return new[] { Issue(typeNameIdentifier, proposal) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}