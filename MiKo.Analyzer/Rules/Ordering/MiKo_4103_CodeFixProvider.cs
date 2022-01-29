using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4103_CodeFixProvider)), Shared]
    public sealed class MiKo_4103_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4103_TestOneTimeSetUpMethodOrderingAnalyzer.Id;

        protected override string Title => Resources.MiKo_4103_CodeFixTitle;

        protected override SyntaxNode GetUpdatedTypeSyntax(CodeFixContext context, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var method = (MethodDeclarationSyntax)syntax;

            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(method);

            var firstMethod = modifiedType.FirstChild<MethodDeclarationSyntax>();

            return modifiedType.InsertNodeBefore(firstMethod, method);
        }
    }
}