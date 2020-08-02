using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1043_CodeFixProvider)), Shared]
    public sealed class MiKo_1043_CodeFixProvider : NamingLocalVariableCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1043_CancellationTokenLocalVariableAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = FindSyntax(syntaxNodes.ToList());
            if (syntax is null)
            {
                return null;
            }

            const string Title = "Name it '" + MiKo_1042_CancellationTokenParameterNameAnalyzer.ExpectedName + "'";

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = semanticModel.GetDeclaredSymbol(syntax, token);
                                                                    const string NewName = MiKo_1043_CancellationTokenLocalVariableAnalyzer.ExpectedName;

                                                                    return new Tuple<ISymbol, string>(symbol, NewName);
                                                                },
                                                            _),
                                     Title);
        }
    }
}