using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1048_CodeFixProvider)), Shared]
    public sealed class MiKo_1048_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1048_ValueConverterSuffixAnalyzer.Id;

        protected override string Title => "Append '" + MiKo_1048_ValueConverterSuffixAnalyzer.Suffix + "' suffix";

        protected override string GetNewName(ISymbol symbol) => MiKo_1048_ValueConverterSuffixAnalyzer.FindBetterName((INamedTypeSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<TypeDeclarationSyntax>().FirstOrDefault();
    }
}