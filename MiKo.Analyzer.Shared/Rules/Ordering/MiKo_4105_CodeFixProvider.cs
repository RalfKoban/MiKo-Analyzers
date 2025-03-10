using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4105_CodeFixProvider)), Shared]
    public sealed class MiKo_4105_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_4105";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<FieldDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic) => PlaceFirst<FieldDeclarationSyntax>(syntax, typeSyntax);
    }
}