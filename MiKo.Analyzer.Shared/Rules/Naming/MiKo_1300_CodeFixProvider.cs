﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1300_CodeFixProvider)), Shared]
    public sealed class MiKo_1300_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.Id;

        protected override string Title => Resources.MiKo_1300_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.FindBetterName((IParameterSymbol)symbol, diagnostic);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<SimpleLambdaExpressionSyntax>().First().Parameter;
    }
}