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
    public class MiKo_3034_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_3034_PropertyChangeEventRaiserAnalyzer.Id;

        protected sealed override string Title => Resources.MiKo_3034_CodeFixTitle;

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().First();

        protected sealed override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var parameter = (ParameterSyntax)syntax;

            var name = SyntaxFactory.ParseName("CallerMemberName");
            var attribute = SyntaxFactory.Attribute(name);
            var attributeList = SyntaxFactory.AttributeList().WithAttributes(SyntaxFactory.SeparatedList(new[] { attribute }));

            return parameter.WithAttributeLists(new SyntaxList<AttributeListSyntax>(attributeList))
                            .WithDefault(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, Diagnostic diagnostic) => WithUsing(root, "System.Runtime.CompilerServices");
    }
}