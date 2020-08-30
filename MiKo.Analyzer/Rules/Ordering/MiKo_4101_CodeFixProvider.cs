using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4101_CodeFixProvider)), Shared]
    public sealed class MiKo_4101_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4101_TestSetUpMethodOrderingAnalyzer.Id;

        protected override string Title => "Place test initialization method directly after one-time methods";

        protected override SyntaxNode GetUpdatedTypeSyntax(BaseTypeDeclarationSyntax syntax)
        {
            var method = syntax.ChildNodes().OfType<MethodDeclarationSyntax>().First(_ => _.IsTestSetUpMethod());

            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var modifiedType = syntax.RemoveNode(method, SyntaxRemoveOptions.KeepNoTrivia)
                                     .WithOpenBraceToken(syntax.OpenBraceToken.WithoutTrivia().WithEndOfLine())
                                     .WithCloseBraceToken(syntax.CloseBraceToken.WithoutTrivia().WithLeadingEndOfLine()
                                                                .WithTrailingTrivia(syntax.CloseBraceToken.TrailingTrivia));

            var otherMethods = modifiedType.ChildNodes().OfType<MethodDeclarationSyntax>().ToList();

            var oneTimeSetup = otherMethods.FirstOrDefault(_ => _.IsTestOneTimeSetUpMethod());
            var oneTimeTearDowns = otherMethods.FirstOrDefault(_ => _.IsTestOneTimeTearDownMethod());

            var precedingNode = oneTimeTearDowns ?? oneTimeSetup;
            if (precedingNode is null)
            {
                // place before all other nodes as there is no one-time method
                return modifiedType.InsertNodeBefore(otherMethods.First(), method);
            }

            // and we have to add the trivia to the method (as the original one belonged to the open brace token which we removed above)
            return modifiedType.InsertNodeAfter(precedingNode, method.WithLeadingEndOfLine());
        }
    }
}