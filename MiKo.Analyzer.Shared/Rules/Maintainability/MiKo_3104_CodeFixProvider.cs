using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3104_CodeFixProvider)), Shared]
    public sealed class MiKo_3104_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3104_CombinatorialTestsAnalyzer.Id;

        protected override string Title => Resources.MiKo_3104_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            return syntaxNodes.OfType<MethodDeclarationSyntax>().First();
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var method = (MethodDeclarationSyntax)syntax;

            foreach (var attributeList in method.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var name = attribute.Name.GetNameOnlyPart();

                    switch (name)
                    {
                        case "Combinatorial":
                        case "CombinatorialAttribute":
                        {
                            var listWithoutAttribute = attributeList.Without(attribute);

                            if (listWithoutAttribute.Attributes.Count == 0)
                            {
                                // we do not need an empty list
                                return method.Without(attributeList);
                            }

                            return method.ReplaceNode(attributeList, listWithoutAttribute);
                        }
                    }
                }
            }

            return method;
        }
    }
}