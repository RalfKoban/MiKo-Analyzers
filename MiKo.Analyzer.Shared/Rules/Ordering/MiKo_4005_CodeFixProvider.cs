using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4005_CodeFixProvider)), Shared]
    public sealed class MiKo_4005_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_4005";

        protected override async Task<SyntaxNode> GetUpdatedTypeSyntaxAsync(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            switch (typeSyntax)
            {
                case ClassDeclarationSyntax c: return c.WithBaseList(await UpdatedBaseListAsync(c.BaseList, c.Identifier, document, cancellationToken).ConfigureAwait(false));
                case RecordDeclarationSyntax r: return r.WithBaseList(await UpdatedBaseListAsync(r.BaseList, r.Identifier, document, cancellationToken).ConfigureAwait(false));
                case StructDeclarationSyntax s: return s.WithBaseList(await UpdatedBaseListAsync(s.BaseList, s.Identifier, document, cancellationToken).ConfigureAwait(false));

                default:
                    return typeSyntax;
            }
        }

        private static async Task<BaseListSyntax> UpdatedBaseListAsync(BaseListSyntax baseList, SyntaxToken identifier, Document document, CancellationToken cancellationToken)
        {
            var interfaceName = "I" + identifier.ValueText.GetNameOnlyPart();

            var baseType = baseList.Types.First();

            var type = await baseType.GetTypeSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            var types = new List<BaseTypeSyntax>();

            if (type?.TypeKind is TypeKind.Class)
            {
                // the base type, if any
                types.Add(baseType);
            }

            // the interface that is responsible for the name of the type
            types.Add(baseList.Types.First(_ => _.Type.GetNameOnlyPart() == interfaceName));

            // all the other types
            types.AddRange(baseList.Types.Except(types));

            return SyntaxFactory.BaseList().AddTypes(types.ToArray());
        }
    }
}