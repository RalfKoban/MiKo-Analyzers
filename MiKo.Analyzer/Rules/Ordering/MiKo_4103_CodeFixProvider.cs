using System.Composition;
using System.Linq;

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

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax syntax, Diagnostic diagnostic)
        {
            var method = syntax.ChildNodes().OfType<MethodDeclarationSyntax>().First(_ => _.IsTestOneTimeSetUpMethod());

            var modifiedType = syntax.RemoveNodeAndAdjustOpenCloseBraces(method);

            var firstMethod = modifiedType.ChildNodes().OfType<MethodDeclarationSyntax>().First();

            return modifiedType.InsertNodeBefore(firstMethod, method);
        }
    }
}