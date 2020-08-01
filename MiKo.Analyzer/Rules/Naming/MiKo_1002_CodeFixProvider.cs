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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1002_CodeFixProvider)), Shared]
    public sealed class MiKo_1002_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1002_EventHandlingMethodParametersAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<ParameterSyntax>().First();

            // TODO: RKN maybe the equivalenceKey "Title" is wrong and should contain the name of the resulting parameter (such as "e" or "sender")
            const string Title = "Rename event argument";

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = semanticModel.GetDeclaredSymbol(syntax, token);
                                                                    var newName = symbol.Type.IsObject()
                                                                                      ? MiKo_1002_EventHandlingMethodParametersAnalyzer.Sender
                                                                                      : MiKo_1002_EventHandlingMethodParametersAnalyzer.EventArgs;

                                                                    return new Tuple<ISymbol, string>(symbol, newName);
                                                                },
                                                            _),
                                     Title);
        }
    }
}