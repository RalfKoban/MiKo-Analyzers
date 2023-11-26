﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1075_CodeFixProvider)), Shared]
    public sealed class MiKo_1075_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer.Id;

        protected override string Title => Resources.MiKo_1075_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1075_TypesSuffixedWithEventArgsInheritFromEventArgsAnalyzer.FindBetterName(symbol, diagnostic);

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ClassDeclarationSyntax>().FirstOrDefault();
    }
}