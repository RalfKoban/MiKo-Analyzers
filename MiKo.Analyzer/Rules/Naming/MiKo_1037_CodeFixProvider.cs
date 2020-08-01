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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1037_CodeFixProvider)), Shared]
    public sealed class MiKo_1037_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1037_EnumSuffixAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var enumSyntax = syntaxNodes.OfType<EnumDeclarationSyntax>().FirstOrDefault();
            if (enumSyntax != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(enumSyntax, t));
            }

            var typeSyntax = syntaxNodes.OfType<TypeDeclarationSyntax>().FirstOrDefault();
            if (typeSyntax != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(typeSyntax, t));
            }

            return null;
        }

        private static CodeAction CreateCodeAction(Document document, Func<SemanticModel, CancellationToken, ITypeSymbol> symbolProvider)
        {
            const string Title = "Remove 'Enum' suffix";

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = symbolProvider(semanticModel, token);
                                                                    var newName = MiKo_1037_EnumSuffixAnalyzer.FindBetterName(symbol);

                                                                    return new Tuple<ISymbol, string>(symbol, newName);
                                                                },
                                                            _),
                                     Title);
        }
    }
}
