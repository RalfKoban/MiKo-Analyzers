using System;
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

            var modifiedParameter = parameter.WithAttributeLists(new SyntaxList<AttributeListSyntax>(attributeList))
                            .WithDefault(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            return modifiedParameter;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax)
        {
            const string ns = "System.Runtime.CompilerServices";

            var usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();
            if (usings.Any(_ => _.Name.ToFullString() == ns))
            {
                // already set
                return root;
            }

            var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns));

            if (usings.Count == 0)
            {
                return root.InsertNodeBefore(root.ChildNodes().First(), directive);
            }

            // add using at correct place inside the using block
            var usingOrientation = usings.FirstOrDefault(_ => string.Compare(_.Name.ToFullString(), ns, StringComparison.OrdinalIgnoreCase) > 0);

            return usingOrientation != null
                       ? root.InsertNodeBefore(usingOrientation, directive)
                       : root.InsertNodeAfter(usings.Last(), directive);
        }
    }
}