using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2091_CodeFixProvider)), Shared]
    public sealed class MiKo_2091_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2091_InequalityOperatorAnalyzer.Id;

        protected override string Title => "Apply standard comment";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var method = comment.AncestorsAndSelf().OfType<OperatorDeclarationSyntax>().First();
            var parameters = method.ParameterList.Parameters;
            var typeDeclarationSyntax = method.Ancestors().OfType<TypeDeclarationSyntax>().First();
            var type = SyntaxFactory.ParseTypeName(typeDeclarationSyntax.Identifier.ValueText);

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), "Determines whether the specified ", SeeCref(type), " instances are considered not equal.").WithTrailingXmlComment();
            var param1 = Comment(SyntaxFactory.XmlParamElement(parameters[0].Identifier.ValueText), "The first value to compare.").WithTrailingXmlComment();
            var param2 = Comment(SyntaxFactory.XmlParamElement(parameters[1].Identifier.ValueText), "The second value to compare.").WithTrailingXmlComment();

            var returns = SyntaxFactory.XmlReturnsElement(
                                                          SeeLangword_True().WithLeadingXmlComment(),
                                                          SyntaxFactory.XmlText(" if both instances are considered not equal; otherwise, "),
                                                          SeeLangword_False(),
                                                          SyntaxFactory.XmlText(".").WithTrailingXmlComment())
                                       .WithEndOfLine();

            return SyntaxFactory.DocumentationComment(summary, param1, param2, returns);
        }
    }
}