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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1105_CodeFixProvider)), Shared]
    public sealed class MiKo_1105_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1105_OneTimeTestSetupMethodsAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = syntaxNodes.OfType<MethodDeclarationSyntax>().First();

            const string Title = "Rename to '" + MiKo_1105_OneTimeTestSetupMethodsAnalyzer.ExpectedName + "'";

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = semanticModel.GetDeclaredSymbol(syntax, token);
                                                                    const string NewName = MiKo_1105_OneTimeTestSetupMethodsAnalyzer.ExpectedName;

                                                                    return new Tuple<ISymbol, string>(symbol, NewName);
                                                                },
                                                            _),
                                     Title);
        }
    }
}