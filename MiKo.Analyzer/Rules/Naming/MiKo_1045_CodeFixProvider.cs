using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1045_CodeFixProvider)), Shared]
    public sealed class MiKo_1045_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1045_CommandInvokeMethodsSuffixAnalyzer.Id;

        protected override string Title => "Remove '" + MiKo_1045_CommandInvokeMethodsSuffixAnalyzer.Suffix + "' suffix";

        protected override string GetNewName(ISymbol symbol) => MiKo_1045_CommandInvokeMethodsSuffixAnalyzer.FindBetterName(symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>().FirstOrDefault();
    }
}