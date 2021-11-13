using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        protected override IEnumerable<XmlNodeSyntax> GetDefaultComment(Document document, TypeSyntax returnType)
        {
            var symbol = GetSymbol(document, returnType);

            if (symbol is INamedTypeSymbol typeSymbol && typeSymbol.IsEnum())
            {
                var defaultValue = typeSymbol.GetMembers().OfType<IFieldSymbol>().First();
                var nameSyntax = SyntaxFactory.ParseName(defaultValue.Name);

                yield return XmlText(Constants.Comments.DefaultStartingPhrase);
                yield return SeeCref(returnType, nameSyntax);
                yield return XmlText(".");
            }
        }
    }
}