using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4106_CodeFixProvider)), Shared]
    public sealed class MiKo_4106_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_4106";

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic issue) => PlaceFirst<MethodDeclarationSyntax>(syntax, typeSyntax);
    }
}