using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1109_CodeFixProvider)), Shared]
    public sealed class MiKo_1109_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer.Id;

        protected override string Title => "Prefix with '" + MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer.Prefix + "' instead of suffix '" + MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer.Suffix + "'";

        protected override string GetNewName(ISymbol symbol) => MiKo_1109_TestableClassesShouldNotBeSuffixedWithUtAnalyzer.FindBetterName((INamedTypeSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<TypeDeclarationSyntax>().FirstOrDefault();
    }
}