using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            switch (typeSyntax)
            {
                case ClassDeclarationSyntax c: return c.WithBaseList(UpdatedBaseList(document, c.BaseList, c.Identifier));
                case RecordDeclarationSyntax r: return r.WithBaseList(UpdatedBaseList(document, r.BaseList, r.Identifier));
                case StructDeclarationSyntax s: return s.WithBaseList(UpdatedBaseList(document, s.BaseList, s.Identifier));

                default:
                    return typeSyntax;
            }
        }

        private static BaseListSyntax UpdatedBaseList(Document document, BaseListSyntax baseList, SyntaxToken identifier)
        {
            var interfaceName = "I" + identifier.ValueText.GetNameOnlyPart();

            var baseType = baseList.Types.First();

            var type = baseType.GetTypeSymbol(GetSemanticModel(document));

            var types = new List<BaseTypeSyntax>();

            if (type?.TypeKind == TypeKind.Class)
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