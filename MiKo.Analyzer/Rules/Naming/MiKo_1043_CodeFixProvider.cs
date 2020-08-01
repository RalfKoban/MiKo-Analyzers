using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1043_CodeFixProvider)), Shared]
    public sealed class MiKo_1043_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1043_CancellationTokenLocalVariableAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var variableSyntax = syntaxNodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            if (variableSyntax != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(variableSyntax, t));
            }

            var variableDesignationSyntax = syntaxNodes.OfType<SingleVariableDesignationSyntax>().FirstOrDefault();
            if (variableDesignationSyntax != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(variableDesignationSyntax, t));
            }

            var forEachStatementSyntax = syntaxNodes.OfType<ForEachStatementSyntax>().FirstOrDefault();
            if (forEachStatementSyntax != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(forEachStatementSyntax, t));
            }

            return null;
        }

        private static CodeAction CreateCodeAction(Document document, Func<SemanticModel, CancellationToken, ISymbol> symbolProvider)
        {
            const string Title = "Name it '" + MiKo_1042_CancellationTokenParameterNameAnalyzer.ExpectedName + "'";

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = symbolProvider(semanticModel, token);

                                                                    const string NewName = MiKo_1043_CancellationTokenLocalVariableAnalyzer.ExpectedName;

                                                                    return new Tuple<ISymbol, string>(symbol, NewName);
                                                                },
                                                            _),
                                     Title);
        }
    }
}