using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2036_Enum_CodeFixProvider)), Shared]
    public sealed class MiKo_2036_Enum_CodeFixProvider : MiKo_2036_CodeFixProvider
    {
        protected override string Title => Resources.MiKo_2036_CodeFixTitle_Enum;

        protected override bool IsApplicable(in ImmutableArray<Diagnostic> issues) => base.IsApplicable(issues) is false;

        protected override async Task<XmlNodeSyntax[]> GetDefaultCommentAsync(TypeSyntax returnType, Document document, CancellationToken cancellationToken)
        {
            var symbol = await returnType.GetSymbolAsync(document, cancellationToken).ConfigureAwait(false);

            return GetDefaultComment(symbol, returnType);
        }

        private static XmlNodeSyntax[] GetDefaultComment(ISymbol symbol, TypeSyntax returnType)
        {
            if (symbol is INamedTypeSymbol typeSymbol && typeSymbol.IsEnum())
            {
                var defaultValue = typeSymbol.GetFields()[0];

                return new XmlNodeSyntax[]
                           {
                               XmlText(Constants.Comments.DefaultStartingPhrase),
                               SeeCref(returnType, defaultValue.Name),
                               XmlText("."),
                           };
            }

            return Array.Empty<XmlNodeSyntax>();
        }
    }
}