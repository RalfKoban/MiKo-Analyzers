using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1079_RepositorySuffixAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1079";

        private const string Repository = nameof(Repository);

        public MiKo_1079_RepositorySuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(string typeName, BaseTypeDeclarationSyntax declaration) => typeName.Length > Repository.Length;

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration)
        {
            return typeName.EndsWith(Repository, StringComparison.Ordinal)
                   ? new[] { Issue(typeNameIdentifier, FindBetterName(typeName)) }
                   : Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string name) => Pluralizer.MakePluralName(name.WithoutSuffix(Repository));
    }
}