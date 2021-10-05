using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4102_CodeFixProvider)), Shared]
    public sealed class MiKo_4102_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4102_TestTearDownMethodOrderingAnalyzer.Id;

        protected override string Title => Resources.MiKo_4102_CodeFixTitle;

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax syntax, Diagnostic diagnostic)
        {
            var method = syntax.ChildNodes().OfType<MethodDeclarationSyntax>().First(_ => _.IsTestTearDownMethod());

            var modifiedType = syntax.RemoveNodeAndAdjustOpenCloseBraces(method);

            var otherMethods = modifiedType.ChildNodes().OfType<MethodDeclarationSyntax>().ToList();

            var precedingNode = otherMethods.FirstOrDefault(_ => _.IsTestSetUpMethod())
                             ?? otherMethods.FirstOrDefault(_ => _.IsTestOneTimeTearDownMethod())
                             ?? otherMethods.FirstOrDefault(_ => _.IsTestOneTimeSetUpMethod());

            if (precedingNode is null)
            {
                // place before all other nodes as there is no set-up or one-time method
                return modifiedType.InsertNodeBefore(otherMethods.First(), method);
            }

            // we have to add the trivia to the method (as the original one belonged to the open brace token which we removed above)
            return modifiedType.InsertNodeAfter(precedingNode, method.WithLeadingEndOfLine());
        }
    }
}