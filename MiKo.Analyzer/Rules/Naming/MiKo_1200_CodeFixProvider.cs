using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1200_CodeFixProvider)), Shared]
    public sealed class MiKo_1200_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1200_ExceptionCatchBlockAnalyzer.Id;

        protected override string Title => Resources.MiKo_1200_CodeFixTitle;

        protected override string GetNewName(ISymbol symbol) => MiKo_1200_ExceptionCatchBlockAnalyzer.ExpectedName;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<CatchDeclarationSyntax>().FirstOrDefault();
    }
}
