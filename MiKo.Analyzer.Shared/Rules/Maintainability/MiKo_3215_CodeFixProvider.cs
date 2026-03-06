using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3215_CodeFixProvider)), Shared]
    public sealed class MiKo_3215_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3215";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                switch (syntaxNode)
                {
                    case ParameterSyntax _:
                    case PropertyDeclarationSyntax _:
                    case FieldDeclarationSyntax _:
                        return syntaxNode;

                    case BaseTypeDeclarationSyntax _:
                        return null;
                }
            }

            return null;
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case ParameterSyntax parameter when parameter.Type is GenericNameSyntax generic:
                    return parameter.WithType(GetTypeSyntax(generic));

                case PropertyDeclarationSyntax property when property.Type is GenericNameSyntax generic:
                    return property.WithType(GetTypeSyntax(generic));

                case FieldDeclarationSyntax field when field.Declaration.Type is GenericNameSyntax generic:
                    return field.WithDeclaration(field.Declaration.WithType(GetTypeSyntax(generic)));

                default:
                    return syntax;
            }
        }

        private static GenericNameSyntax GetTypeSyntax(GenericNameSyntax generic) => GenericName("Func", generic.TypeArgumentList.AddArguments(PredefinedType(SyntaxKind.BoolKeyword)));
    }
}