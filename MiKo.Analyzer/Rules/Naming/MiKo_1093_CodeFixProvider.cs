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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1093_CodeFixProvider)), Shared]
    public sealed class MiKo_1093_CodeFixProvider : NamingCodeFixProvider
    {
        private static readonly string Title = "Remove suffix " + MiKo_1093_ObjectSuffixAnalyzer.WrongSuffixes.HumanizedConcatenated();

        public override string FixableDiagnosticId => MiKo_1093_ObjectSuffixAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var nodes = syntaxNodes.ToList();

            var field = nodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            if (field != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(field, t));
            }

            var property = nodes.OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            if (property != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(property, t));
            }

            var typeDeclarationSyntax = nodes.OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
            if (typeDeclarationSyntax != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(typeDeclarationSyntax, t));
            }

            var namespaceDeclarationSyntax = nodes.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            if (namespaceDeclarationSyntax != null)
            {
                return CreateCodeAction(document, (s, t) => s.GetDeclaredSymbol(namespaceDeclarationSyntax, t));
            }

            return null;
        }

        private static CodeAction CreateCodeAction(Document document, Func<SemanticModel, CancellationToken, ISymbol> symbolProvider)
        {
            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = symbolProvider(semanticModel, token);
                                                                    var newName = MiKo_1093_ObjectSuffixAnalyzer.FindBetterName(symbol);

                                                                    return new Tuple<ISymbol, string>(symbol, newName);
                                                                },
                                                            _),
                                     Title);
        }
    }
}