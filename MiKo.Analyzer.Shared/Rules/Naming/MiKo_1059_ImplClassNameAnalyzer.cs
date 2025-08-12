using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1059_ImplClassNameAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1059";

        private static readonly string[] WrongSuffixes = { "Impl", "Implementation", "Imp" };

        public MiKo_1059_ImplClassNameAnalyzer() : base(Id)
        {
        }

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            foreach (var wrongSuffix in WrongSuffixes)
            {
                if (typeName.EndsWith(wrongSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    var proposal = typeName.WithoutSuffix(wrongSuffix);

                    return new[] { Issue(typeNameIdentifier, proposal, wrongSuffix) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}