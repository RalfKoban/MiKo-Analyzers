using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1055_1056_CodeFixProvider)), Shared]
    public sealed class MiKo_1055_1056_CodeFixProvider : NamingCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
                                                                                         MiKo_1055_DependencyPropertyFieldSuffixAnalyzer.Id,
                                                                                         MiKo_1056_DependencyPropertyFieldPrefixAnalyzer.Id);

        public override string FixableDiagnosticId => "MiKo_1055_1056";

        protected override string Title => Resources.MiKo_1055_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
    }
}