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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1004_CodeFixProvider)), Shared]
    public sealed class MiKo_1004_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1004_EventNameSuffixAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = FindEventSyntax(syntaxNodes.ToList());

            const string Title = "Remove '" + MiKo_1004_EventNameSuffixAnalyzer.Suffix + "' suffix";

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = semanticModel.GetDeclaredSymbol(syntax, token);
                                                                    var newName = MiKo_1004_EventNameSuffixAnalyzer.FindBetterName(symbol);

                                                                    return new Tuple<ISymbol, string>(symbol, newName);
                                                                },
                                                            _),
                                     Title);
        }

        private static SyntaxNode FindEventSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            SyntaxNode eventDeclarationSyntax = syntaxNodes.OfType<EventDeclarationSyntax>().FirstOrDefault();
            SyntaxNode variableDeclaratorSyntax = syntaxNodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();

            return eventDeclarationSyntax ?? variableDeclaratorSyntax;
        }
    }
}