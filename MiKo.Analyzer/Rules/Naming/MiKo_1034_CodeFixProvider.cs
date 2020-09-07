using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1034_CodeFixProvider)), Shared]
    public sealed class MiKo_1034_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1034_FieldModelSuffixAnalyzer.Id;

        protected override string Title => Resources.MiKo_1034_CodeFixTitle;

        protected override string GetNewName(ISymbol symbol) => MiKo_1034_FieldModelSuffixAnalyzer.FindBetterName((IFieldSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
    }
}