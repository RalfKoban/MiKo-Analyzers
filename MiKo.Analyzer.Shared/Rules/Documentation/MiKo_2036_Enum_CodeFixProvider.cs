using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2036_Enum_CodeFixProvider)), Shared]
    public sealed class MiKo_2036_Enum_CodeFixProvider : MiKo_2036_CodeFixProvider
    {
        protected override string Title => Resources.MiKo_2036_CodeFixTitle_Enum;

        protected override bool IsApplicable(in ImmutableArray<Diagnostic> issues) => base.IsApplicable(issues) is false;

        protected override IEnumerable<XmlNodeSyntax> GetDefaultComment(Document document, TypeSyntax returnType)
        {
            var symbol = returnType.GetSymbol(document);

            if (symbol is INamedTypeSymbol typeSymbol && typeSymbol.IsEnum())
            {
                var defaultValue = typeSymbol.GetFields()[0];
                var nameSyntax = SyntaxFactory.ParseName(defaultValue.Name);

                return new XmlNodeSyntax[]
                           {
                               XmlText(Constants.Comments.DefaultStartingPhrase),
                               SeeCref(returnType, nameSyntax),
                               XmlText("."),
                           };
            }

            return Array.Empty<XmlNodeSyntax>();
        }
    }
}