using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2091_CodeFixProvider)), Shared]
    public sealed class MiKo_2091_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2091";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var method = syntax.FirstAncestorOrSelf<OperatorDeclarationSyntax>();

            if (method is null)
            {
                return syntax;
            }

            var parameters = method.ParameterList.Parameters;
            var typeDeclarationSyntax = method.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            var type = SyntaxFactory.ParseTypeName(typeDeclarationSyntax?.Identifier.ValueText ?? string.Empty);

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), "Determines whether the specified ", SeeCref(type), " instances are considered not equal.").WithTrailingXmlComment();
            var param1 = ParameterComment(parameters[0], "The first value to compare.").WithTrailingXmlComment();
            var param2 = ParameterComment(parameters[1], "The second value to compare.").WithTrailingXmlComment();

            var returns = SyntaxFactory.XmlReturnsElement(
                                                      SeeLangword_True().WithLeadingXmlComment(),
                                                      XmlText(" if both instances are considered not equal; otherwise, "),
                                                      SeeLangword_False(),
                                                      XmlText(".").WithTrailingXmlComment())
                                       .WithEndOfLine();

            return SyntaxFactory.DocumentationComment(summary, param1, param2, returns);
        }
    }
}