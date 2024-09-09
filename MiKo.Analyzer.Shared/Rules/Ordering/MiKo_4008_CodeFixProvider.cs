using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4008_CodeFixProvider)), Shared]
    public sealed class MiKo_4008_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_4008";

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(syntax);
            var method = modifiedType.ChildNodes<MethodDeclarationSyntax>().Last(_ => _.GetName() == nameof(Equals) && _.Modifiers.Any(SyntaxKind.PublicKeyword));

            // place after equals method
            return modifiedType.InsertNodeAfter(method, syntax);
        }
    }
}