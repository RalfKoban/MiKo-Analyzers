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

        public override string FixableDiagnosticId => "MiKo_1057_1058";

        protected override string Title => Resources.MiKo_1057_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
    }
}