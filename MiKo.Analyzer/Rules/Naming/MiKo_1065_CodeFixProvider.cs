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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1065_CodeFixProvider)), Shared]
    public sealed class MiKo_1065_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1065_OperatorParameterNameAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<ParameterSyntax>().First();

            var title = "Rename parameter '" + syntax.GetName() + "'";

            return CodeAction.Create(
                                     title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = semanticModel.GetDeclaredSymbol(syntax, token);
                                                                    var newName = MiKo_1065_OperatorParameterNameAnalyzer.FindBetterName(symbol);

                                                                    return new Tuple<ISymbol, string>(symbol, newName);
                                                                },
                                                            _),
                                     "Rename operator parameter");
        }
    }
}