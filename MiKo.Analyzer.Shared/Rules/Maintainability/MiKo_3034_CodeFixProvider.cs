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
        public override string FixableDiagnosticId => "MiKo_3034";

        protected override string Title => Resources.MiKo_3034_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var parameter = (ParameterSyntax)syntax;

            var name = SyntaxFactory.ParseName("CallerMemberName");
            var attribute = SyntaxFactory.Attribute(name);
            var attributeList = SyntaxFactory.AttributeList(new[] { attribute }.ToSeparatedSyntaxList());

            return parameter.WithAttributeLists(new SyntaxList<AttributeListSyntax>(attributeList))
                            .WithDefault(SyntaxFactory.EqualsValueClause(NullLiteral()));
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue) => root.WithUsing("System.Runtime.CompilerServices");
    }
}