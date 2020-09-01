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

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax syntax)
        {
            var method = syntax.ChildNodes().OfType<MethodDeclarationSyntax>().First(_ => _.IsTestOneTimeTearDownMethod());

            var modifiedType = syntax.RemoveNodeAndAdjustOpenCloseBraces(method);

            var firstMethod = modifiedType.ChildNodes().OfType<MethodDeclarationSyntax>().First();

            if (firstMethod.IsTestOneTimeSetUpMethod())
            {
                // we have to add the trivia to the method (as the original one belonged to the open brace token which we removed above)
                return modifiedType.InsertNodeAfter(firstMethod, method.WithLeadingEndOfLine());
            }

            return modifiedType.InsertNodeBefore(firstMethod, method);
        }
    }
}