using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1057_1058_CodeFixProvider)), Shared]
    public sealed class MiKo_1057_1058_CodeFixProvider : NamingCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
                                                                                             MiKo_1057_DependencyPropertyKeyFieldSuffixAnalyzer.Id,
                                                                                             MiKo_1058_DependencyPropertyKeyFieldPrefixAnalyzer.Id);

        public override string FixableDiagnosticId => MiKo_1056_DependencyPropertyFieldPrefixAnalyzer.Id;

        protected override string Title => "Rename dependency property key";

        protected override string GetNewName(ISymbol symbol) => MiKo_1058_DependencyPropertyKeyFieldPrefixAnalyzer.FindBetterName((IFieldSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
    }
}