using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3128_CodeFixProvider)), Shared]
    public sealed class MiKo_3128_CodeFixProvider : UnitTestCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3128";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>().First();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is MethodDeclarationSyntax method && method.ParameterList.Parameters.Count is 0)
            {
                var name = issue.Properties[Constants.AnalyzerCodeFixSharedData.Marker];
                var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(name));
                var attributeList = SyntaxFactory.AttributeList(attribute.ToSeparatedSyntaxList()).WithEndOfLine();

                return method.WithAttributeLists(method.AttributeLists.Add(attributeList));
            }

            return syntax;
        }
    }
}