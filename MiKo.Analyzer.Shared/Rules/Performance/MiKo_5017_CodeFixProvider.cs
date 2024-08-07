using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5017_CodeFixProvider)), Shared]
    public sealed class MiKo_5017_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_5017";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case FieldDeclarationSyntax _:
                    case LocalDeclarationStatementSyntax _:
                        return node;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case FieldDeclarationSyntax field:
                {
                    var modifiers = field.Modifiers.Select(_ => _.Kind()).ToList();
                    modifiers.Remove(SyntaxKind.StaticKeyword);
                    modifiers.Remove(SyntaxKind.ReadOnlyKeyword);
                    modifiers.Add(SyntaxKind.ConstKeyword);

                    return field.WithModifiers(modifiers);
                }

                case LocalDeclarationStatementSyntax declaration:
                {
                    var variable = declaration.Declaration;

                    return declaration.AddModifiers(SyntaxKind.ConstKeyword.AsToken()).WithLeadingTriviaFrom(variable.Type)
                                      .WithDeclaration(variable.WithType(PredefinedType(SyntaxKind.StringKeyword)));
                }

                default:
                    return null;
            }
        }
    }
}