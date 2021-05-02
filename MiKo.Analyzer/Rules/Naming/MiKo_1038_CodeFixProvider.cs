using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1038_CodeFixProvider)), Shared]
    public sealed class MiKo_1038_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1038_ExtensionMethodsClassSuffixAnalyzer.Id;

        protected override string Title => "Suffix type with '" + MiKo_1038_ExtensionMethodsClassSuffixAnalyzer.Suffix + "'";

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1038_ExtensionMethodsClassSuffixAnalyzer.FindBetterName((INamedTypeSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<TypeDeclarationSyntax>().FirstOrDefault();
    }
}