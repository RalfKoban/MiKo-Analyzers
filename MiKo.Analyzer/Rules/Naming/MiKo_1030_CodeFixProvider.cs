using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1030_CodeFixProvider)), Shared]
    public sealed class MiKo_1030_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1030_BaseTypePrefixSuffixAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<TypeDeclarationSyntax>().First();

            const string Title = "Remove base type indicator";

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = semanticModel.GetDeclaredSymbol(syntax, token);
                                                                    var newName = MiKo_1030_BaseTypePrefixSuffixAnalyzer.FindBetterName(symbol);

                                                                    return new Tuple<ISymbol, string>(symbol, newName);
                                                                },
                                                            _),
                                     Title);
        }
    }
}