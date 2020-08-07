﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1037_CodeFixProvider)), Shared]
    public sealed class MiKo_1037_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1037_EnumSuffixAnalyzer.Id;

        protected override string Title => "Remove 'Enum' suffix";

        protected override string GetNewName(ISymbol symbol) => MiKo_1037_EnumSuffixAnalyzer.FindBetterName((INamedTypeSymbol)symbol);

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
    }
}
