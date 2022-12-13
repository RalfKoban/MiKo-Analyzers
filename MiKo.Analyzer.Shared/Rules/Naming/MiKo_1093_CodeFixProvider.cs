using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1093_CodeFixProvider)), Shared]
    public sealed class MiKo_1093_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1093_ObjectSuffixAnalyzer.Id;

        protected override string Title => Resources.MiKo_1093_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1093_ObjectSuffixAnalyzer.FindBetterName(symbol);

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                switch (syntaxNode)
                {
                    case VariableDeclaratorSyntax v: return v;
                    case PropertyDeclarationSyntax p: return p;
                    case BaseTypeDeclarationSyntax b: return b;
                    case NamespaceDeclarationSyntax n: return n;
                }
            }

            return null;
        }
    }
}