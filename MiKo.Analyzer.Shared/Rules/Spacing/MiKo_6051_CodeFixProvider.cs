using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6051_CodeFixProvider)), Shared]
    public sealed class MiKo_6051_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6051";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ConstructorDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ConstructorDeclarationSyntax ctor)
            {
                var initializer = ctor.Initializer;

                if (initializer != null)
                {
                    var keyword = initializer.ThisOrBaseKeyword;

                    var updatedToken = initializer.ColonToken.WithLeadingEndOfLine().WithAdditionalLeadingTriviaFrom(keyword).WithTrailingSpace();
                    var updatedKeyword = keyword.WithoutLeadingTrivia();

                    var updatedInitializer = initializer.WithColonToken(updatedToken).WithThisOrBaseKeyword(updatedKeyword);

                    return ctor.WithParameterList(ctor.ParameterList.WithoutTrailingTrivia())
                               .WithInitializer(updatedInitializer);
                }
            }

            return syntax;
        }
    }
}