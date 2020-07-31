using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1300_SimpleLambdaExpressionIdentifierCodeFixProvider)), Shared]
    public sealed class MiKo_1300_SimpleLambdaExpressionIdentifierCodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<SimpleLambdaExpressionSyntax>().First().Parameter;

            const string NewName = Constants.LambdaIdentifiers.Default;
            const string Title = "Name it '" + NewName + "'";

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = semanticModel.GetDeclaredSymbol(syntax, token);

                                                                    return new Tuple<ISymbol, string>(symbol, NewName);
                                                                },
                                                            _),
                                     Title);
        }
    }
}