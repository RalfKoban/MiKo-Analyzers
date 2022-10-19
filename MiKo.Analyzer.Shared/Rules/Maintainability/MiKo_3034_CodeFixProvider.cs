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

        protected sealed override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var parameter = (ParameterSyntax)syntax;

            var name = SyntaxFactory.ParseName("CallerMemberName");
            var attribute = SyntaxFactory.Attribute(name);
            var attributeList = SyntaxFactory.AttributeList().WithAttributes(SyntaxFactory.SeparatedList(new[] { attribute }));

            return parameter.WithAttributeLists(new SyntaxList<AttributeListSyntax>(attributeList))
                            .WithDefault(SyntaxFactory.EqualsValueClause(Literal(SyntaxKind.NullLiteralExpression)));
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(CodeFixContext context, SyntaxNode root, SyntaxNode syntax, Diagnostic issue) => root.WithUsing("System.Runtime.CompilerServices");
    }
}