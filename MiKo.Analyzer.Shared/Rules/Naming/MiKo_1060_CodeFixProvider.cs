using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1060_CodeFixProvider)), Shared]
    public sealed class MiKo_1060_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1060";

        protected override string Title => Resources.MiKo_1060_CodeFixTitle;

        protected override string GetNewName(Diagnostic diagnostic, ISymbol symbol) => MiKo_1060_UseNotFoundInsteadOfMissingAnalyzer.FindBetterName(symbol, diagnostic);

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                if (node is EnumMemberDeclarationSyntax)
                {
                    return node;
                }

                if (node is BaseTypeDeclarationSyntax)
                {
                    return node;
                }
            }

            return null;
        }
    }
}