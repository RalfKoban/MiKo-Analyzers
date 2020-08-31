using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4104_CodeFixProvider)), Shared]
    public sealed class MiKo_4104_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4104_TestOneTimeTearDownMethodOrderingAnalyzer.Id;

        protected override string Title => Resources.MiKo_4104_MessageFormat;

        protected override SyntaxNode GetUpdatedTypeSyntax(BaseTypeDeclarationSyntax syntax)
        {
            var method = syntax.ChildNodes().OfType<MethodDeclarationSyntax>().First(_ => _.IsTestOneTimeTearDownMethod());

            var modifiedType = syntax.RemoveNode(method, SyntaxRemoveOptions.KeepNoTrivia);

            var firstMethod = modifiedType.ChildNodes().OfType<MethodDeclarationSyntax>().First();
            if (firstMethod.IsTestOneTimeSetUpMethod())
            {
                // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
                modifiedType = modifiedType.WithOpenBraceToken(modifiedType.OpenBraceToken.WithoutTrivia().WithEndOfLine());

                // but have to search for the items again
                firstMethod = modifiedType.ChildNodes().OfType<MethodDeclarationSyntax>().First();

                // and we have to add the trivia to the method (as the original one belonged to the open brace token which we removed above)
                return modifiedType.InsertNodeAfter(firstMethod, method.WithLeadingEndOfLine());
            }

            return modifiedType.InsertNodeBefore(firstMethod, method);
        }
    }
}