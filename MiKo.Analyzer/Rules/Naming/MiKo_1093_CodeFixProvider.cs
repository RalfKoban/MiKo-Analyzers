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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1093_CodeFixProvider)), Shared]
    public sealed class MiKo_1093_CodeFixProvider : NamingCodeFixProvider
    {
        private static readonly string Title = "Remove suffix " + MiKo_1093_ObjectSuffixAnalyzer.WrongSuffixes.HumanizedConcatenated();

        public override string FixableDiagnosticId => MiKo_1093_ObjectSuffixAnalyzer.Id;

        protected override CodeAction CreateCodeAction(Document document, IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = FindSyntax(syntaxNodes.ToList());
            if (syntax is null)
            {
                return null;
            }

            return CodeAction.Create(
                                     Title,
                                     _ => RenameSymbolAsync(
                                                            document,
                                                            (semanticModel, token) =>
                                                                {
                                                                    var symbol = semanticModel.GetDeclaredSymbol(syntax, token);
                                                                    var newName = MiKo_1093_ObjectSuffixAnalyzer.FindBetterName(symbol);

                                                                    return new Tuple<ISymbol, string>(symbol, newName);
                                                                },
                                                            _),
                                     Title);
        }

        private static SyntaxNode FindSyntax(IReadOnlyCollection<SyntaxNode> nodes)
        {
            var field = nodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            if (field != null)
            {
                return field;
            }

            var property = nodes.OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            if (property != null)
            {
                return property;
            }

            var typeDeclarationSyntax = nodes.OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
            if (typeDeclarationSyntax != null)
            {
                return typeDeclarationSyntax;
            }

            var namespaceDeclarationSyntax = nodes.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            if (namespaceDeclarationSyntax != null)
            {
                return namespaceDeclarationSyntax;
            }

            return null;
        }
    }
}