using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4007_CodeFixProvider)), Shared]
    public sealed class MiKo_4007_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_4007";

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(syntax);
            var method = modifiedType.FirstChild<MethodDeclarationSyntax>();

            // place before all other methods
            return modifiedType.InsertNodeBefore(method, syntax);
        }
    }
}