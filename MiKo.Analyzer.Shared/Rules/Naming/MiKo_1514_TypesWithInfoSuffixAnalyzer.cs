using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1514_TypesWithInfoSuffixAnalyzer : TypeSyntaxNamingAnalyzer
    {
        public const string Id = "MiKo_1514";

        public MiKo_1514_TypesWithInfoSuffixAnalyzer() : base(Id)
        {
        }

        protected override Diagnostic[] AnalyzeName(string typeName, in SyntaxToken typeNameIdentifier, BaseTypeDeclarationSyntax declaration) => typeName.EndsWith("Info", StringComparison.Ordinal)
                                                                                                                                                  ? new[] { Issue(typeNameIdentifier) }
                                                                                                                                                  : Array.Empty<Diagnostic>();
    }
}