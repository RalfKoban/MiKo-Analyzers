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

        protected override bool IsApplicable(IEnumerable<Diagnostic> diagnostics) => base.IsApplicable(diagnostics) is false;

        protected override IEnumerable<XmlNodeSyntax> GetDefaultComment(CodeFixContext context, TypeSyntax returnType)
        {
            var symbol = GetSymbol(context, returnType);

            if (symbol is INamedTypeSymbol typeSymbol && typeSymbol.IsEnum())
            {
                var defaultValue = typeSymbol.GetFields().First();
                var nameSyntax = SyntaxFactory.ParseName(defaultValue.Name);

                yield return XmlText(Constants.Comments.DefaultStartingPhrase);
                yield return SeeCref(returnType, nameSyntax);
                yield return XmlText(".");
            }
        }
    }
}