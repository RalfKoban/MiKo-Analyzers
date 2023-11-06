using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3034_CodeFixProvider)), Shared]
    public sealed class MiKo_3034_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3034_PropertyChangeEventRaiserAnalyzer.Id;

        protected override string Title => Resources.MiKo_3034_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var parameter = (ParameterSyntax)syntax;

            var name = SyntaxFactory.ParseName("CallerMemberName");
            var attribute = SyntaxFactory.Attribute(name);
            var attributeList = SyntaxFactory.AttributeList().WithAttributes(SyntaxFactory.SeparatedList(new[] { attribute }));

            return parameter.WithAttributeLists(new SyntaxList<AttributeListSyntax>(attributeList))
                            .WithDefault(SyntaxFactory.EqualsValueClause(NullLiteral()));
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue) => root.WithUsing("System.Runtime.CompilerServices");
    }
}