﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1054_CodeFixProvider)), Shared]
    public sealed class MiKo_1054_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1054_HelperClassNameAnalyzer.Id;

        protected override string Title => Resources.MiKo_1054_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol)
        {
            if (diagnostic.Properties.TryGetValue(MiKo_1054_HelperClassNameAnalyzer.WrongSuffixIndicator, out var wrongSuffix))
            {
                return symbol.Name.WithoutSuffix(wrongSuffix);
            }

            return symbol.Name;
        }

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            return syntaxNodes.OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
        }
    }
}