using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2090_CodeFixProvider)), Shared]
    public sealed class MiKo_2090_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2090_EqualityOperatorAnalyzer.Id;

        protected override string Title => "Apply default documentation";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var method = comment.AncestorsAndSelf().OfType<OperatorDeclarationSyntax>().First();
            var parameters = method.ParameterList.Parameters;
            var typeDeclarationSyntax = method.Ancestors().OfType<TypeDeclarationSyntax>().First();
            var type = SyntaxFactory.ParseTypeName(typeDeclarationSyntax.Identifier.ValueText);

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), "Determines whether the specified ", SeeCref(type), " instances are considered equal.").WithTrailingXmlComment();
            var param1 = Comment(SyntaxFactory.XmlParamElement(parameters[0].Identifier.ValueText), "The first value to compare.").WithTrailingXmlComment();
            var param2 = Comment(SyntaxFactory.XmlParamElement(parameters[1].Identifier.ValueText), "The second value to compare.").WithTrailingXmlComment();

            var returns = SyntaxFactory.XmlReturnsElement(
                                                          SeeLangword("true").WithLeadingXmlComment(),
                                                          SyntaxFactory.XmlText(" if both instances are considered equal; otherwise, "),
                                                          SeeLangword("false"),
                                                          SyntaxFactory.XmlText(".").WithTrailingXmlComment())
                                       .WithEndOfLine();

            return SyntaxFactory.DocumentationComment(summary, param1, param2, returns);
        }
    }
}