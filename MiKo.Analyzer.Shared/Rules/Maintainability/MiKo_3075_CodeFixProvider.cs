using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3075_CodeFixProvider)), Shared]
    public sealed class MiKo_3075_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3075";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var syntaxNode in syntaxNodes)
            {
                switch (syntaxNode)
                {
                    case ClassDeclarationSyntax c: return c;
                    case RecordDeclarationSyntax r: return r;
                    default:
                        return null;
                }
            }

            return null;
        }

        protected override async Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            switch (syntax)
            {
                case ClassDeclarationSyntax classDeclaration:
                {
                    var makeStatic = await MakeStaticAsync(classDeclaration, document, cancellationToken).ConfigureAwait(false);

                    var keyword = makeStatic
                                  ? SyntaxKind.StaticKeyword
                                  : SyntaxKind.SealedKeyword;

                    return classDeclaration.WithAdditionalModifier(keyword);
                }

                case RecordDeclarationSyntax recordDeclaration:
                {
                    return recordDeclaration.WithAdditionalModifier(SyntaxKind.SealedKeyword);
                }

                default:
                {
                    return syntax;
                }
            }
        }

        private static async Task<bool> MakeStaticAsync(ClassDeclarationSyntax syntax, Document document, CancellationToken cancellationToken)
        {
            var type = await syntax.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            // Inspect members, if all are static, then make it static, else make it sealed
            if (type.BaseType is null || type.BaseType.IsObject())
            {
                return type.GetMembers().All(_ => _.IsStatic || _.GetSyntax() is null);
            }

            return false;
        }
    }
}