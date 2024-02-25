﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1082_CodeFixProvider)), Shared]
    public sealed class MiKo_1082_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1082";

        protected override string Title => Resources.MiKo_1082_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1082_PropertiesWithNumberSuffixAnalyzer.FindBetterName((IPropertySymbol)symbol);

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<PropertyDeclarationSyntax>().FirstOrDefault();
    }
}