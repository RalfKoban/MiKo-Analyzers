using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1059_CodeFixProvider)), Shared]
    public sealed class MiKo_1059_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1059_ImplClassNameAnalyzer.Id;

        protected override string Title => Resources.MiKo_1059_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol)
        {
            var symbolName = symbol.Name;
            if (diagnostic.Properties.TryGetValue(MiKo_1059_ImplClassNameAnalyzer.WrongSuffixIndicator, out var wrongSuffix))
            {
                return symbolName.WithoutSuffix(wrongSuffix);
            }

            return symbolName;
        }

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            return syntaxNodes.OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
        }
    }
}