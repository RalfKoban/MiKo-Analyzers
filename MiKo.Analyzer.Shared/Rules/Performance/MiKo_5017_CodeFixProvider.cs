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
        public override string FixableDiagnosticId => MiKo_5017_StringLiteralVariableAssignmentIsConstantAnalyzer.Id;

        protected override string Title => Resources.MiKo_5017_CodeFixTitle;

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
                    var modifiers = field.Modifiers
                                         .Where(_ => _.IsKind(SyntaxKind.StaticKeyword) is false && _.IsKind(SyntaxKind.ReadOnlyKeyword) is false)
                                         .Append(Token(SyntaxKind.ConstKeyword));

                    return field.WithModifiers(TokenList(modifiers));
                }

                case LocalDeclarationStatementSyntax declaration:
                {
                    var variable = declaration.Declaration;

                    return declaration.AddModifiers(Token(SyntaxKind.ConstKeyword).WithLeadingTriviaFrom(variable.Type))
                                      .WithDeclaration(variable.WithType(PredefinedType(SyntaxKind.StringKeyword)));
                }

                default:
                    return null;
            }
        }
    }
}