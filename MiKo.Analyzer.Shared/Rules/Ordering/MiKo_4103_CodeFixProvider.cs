using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4103_CodeFixProvider)), Shared]
    public sealed class MiKo_4103_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_4103";

        protected override Task<SyntaxNode> GetUpdatedTypeSyntaxAsync(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            SyntaxNode updatedSyntax = GetUpdatedTypeSyntax(typeSyntax, (MethodDeclarationSyntax)syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static BaseTypeDeclarationSyntax GetUpdatedTypeSyntax(BaseTypeDeclarationSyntax typeSyntax, MethodDeclarationSyntax method)
        {
            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(method);

            var otherMethods = modifiedType.ChildNodes<MethodDeclarationSyntax>().ToList();

            var precedingNode = otherMethods.Find(_ => _.IsTestAssemblyWideTearDownMethod())
                             ?? otherMethods.Find(_ => _.IsTestAssemblyWideSetUpMethod());

            if (precedingNode is null)
            {
                // place before all other nodes as there is no assembly-wide method
                return modifiedType.InsertNodeBefore(otherMethods[0], method);
            }

            // we have to add the trivia to the method (as the original one belonged to the open brace token which we removed above)
            return modifiedType.InsertNodeAfter(precedingNode, method.WithLeadingEndOfLine());
        }
    }
}