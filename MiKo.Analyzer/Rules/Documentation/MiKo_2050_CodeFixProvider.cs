using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2050_CodeFixProvider)), Shared]
    public sealed class MiKo_2050_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2050_ExceptionSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2050_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var ctor = comment.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().FirstOrDefault();
            if (ctor != null)
            {
                return FixCtorComment(ctor);
            }
            else
            {
                // we have only the type
                return FixTypeSummary(comment);
            }
        }

        private static SyntaxNode FixTypeSummary(DocumentationCommentTriviaSyntax comment)
        {
            const string Phrase = Constants.Comments.ExceptionTypeSummaryStartingPhrase;

            var summary = GetXmlSyntax(Constants.XmlTag.Summary, comment).FirstOrDefault();
            if (summary is null)
            {
                var newSummary = Comment(SyntaxFactory.XmlSummaryElement(), Phrase).WithTrailingXmlComment();
                return comment.InsertNodeAfter(comment.Content[0], newSummary);
            }
            else
            {
                var newSummary = CommentStartingWith(summary, Phrase);
                return comment.ReplaceNode(summary, newSummary);
            }
        }

        private static SyntaxNode FixCtorComment(ConstructorDeclarationSyntax ctor)
        {
            var typeDeclarationSyntax = ctor.Ancestors().OfType<TypeDeclarationSyntax>().First();
            var type = SyntaxFactory.ParseTypeName(typeDeclarationSyntax.Identifier.ValueText);

            // we have a ctor
            var parameters = ctor.ParameterList.Parameters;

            switch (parameters.Count)
            {
                case 0:
                    return FixParameterlessCtor(type);

                case 1 when parameters[0].Type.IsString():
                    return FixMessageParamCtor(type, parameters[0]);

                case 2 when parameters[0].Type.IsString() && parameters[1].Type.IsException():
                    return FixMessageExceptionParamCtor(type, parameters[0], parameters[1]);

                case 2 when parameters[0].Type.IsSerializationInfo() && parameters[1].Type.IsStreamingContext():
                    return FixSerializationParamCtor(type, parameters[0], parameters[1]);
            }

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

        private static SyntaxNode FixParameterlessCtor(TypeSyntax type)
        {
            var parts = string.Format(Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate + ".", '|').Split('|');

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), parts[0], SeeCref(type), parts[1]);

            return SyntaxFactory.DocumentationComment(summary.WithEndOfLine());
        }

        private static SyntaxNode FixMessageParamCtor(TypeSyntax type, ParameterSyntax messageParameter)
        {
            const string Template = Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate + Constants.Comments.ExceptionCtorMessageParamSummaryContinueingPhrase + ".";

            var parts = string.Format(Template, '|').Split('|');

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), parts[0], SeeCref(type), parts[1]);
            var param = MessageParameterComment(messageParameter);

            return SyntaxFactory.DocumentationComment(
                                                  summary.WithTrailingXmlComment(),
                                                  param.WithEndOfLine());
        }

        private static SyntaxNode FixMessageExceptionParamCtor(TypeSyntax type, ParameterSyntax messageParameter, ParameterSyntax exceptionParameter)
        {
            const string Template = Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate
                                    + Constants.Comments.ExceptionCtorMessageParamSummaryContinueingPhrase
                                    + Constants.Comments.ExceptionCtorExceptionParamSummaryContinueingPhrase
                                    + ".";

            var summaryParts = string.Format(Template, '|').Split('|');

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), summaryParts[0], SeeCref(type), summaryParts[1]);
            var param1 = MessageParameterComment(messageParameter);
            var param2 = ExceptionParameterComment(exceptionParameter);

            return SyntaxFactory.DocumentationComment(
                                                      summary.WithTrailingXmlComment(),
                                                      param1.WithTrailingXmlComment(),
                                                      param2.WithEndOfLine());
        }

        private static SyntaxNode FixSerializationParamCtor(TypeSyntax type, ParameterSyntax serializationInfoParameter, ParameterSyntax streamingContextParameter)
        {
            const string Template = Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate
                                    + Constants.Comments.ExceptionCtorSerializationParamSummaryContinueingPhrase
                                    + ".";

            var summaryParts = string.Format(Template, '|').Split('|');

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), summaryParts[0], SeeCref(type), summaryParts[1]);

            var param1 = ParameterComment(serializationInfoParameter, Constants.Comments.CtorSerializationInfoParamPhrase);
            var param2 = ParameterComment(streamingContextParameter, Constants.Comments.CtorStreamingContextParamPhrase);

            var remarks = Comment(SyntaxFactory.XmlRemarksElement(), Constants.Comments.ExceptionCtorSerializationParamRemarksPhrase);

            return SyntaxFactory.DocumentationComment(
                                                      summary.WithTrailingXmlComment(),
                                                      param1.WithTrailingXmlComment(),
                                                      param2.WithTrailingXmlComment(),
                                                      remarks.WithEndOfLine());
        }

        private static XmlElementSyntax ExceptionParameterComment(ParameterSyntax exceptionParameter)
        {
            var parameterName = exceptionParameter.Identifier.ValueText;

            var parts = string.Format(Constants.Comments.ExceptionCtorExceptionParamPhraseTemplate, '|', '|', '|', '|').Split('|');

            var catchBlock = SyntaxFactory.XmlElement("b", new SyntaxList<XmlNodeSyntax>(SyntaxFactory.XmlText("catch")));
            var paramRef = SyntaxFactory.XmlParamRefElement(parameterName);

            return Comment(
                           SyntaxFactory.XmlParamElement(parameterName),
                           SyntaxFactory.XmlText(parts[0]),
                           Para().WithLeadingXmlComment().WithTrailingXmlComment(),
                           SyntaxFactory.XmlText(parts[1]),
                           paramRef,
                           SyntaxFactory.XmlText(parts[2]),
                           SeeLangword_Null(),
                           SyntaxFactory.XmlText(parts[3]),
                           catchBlock,
                           SyntaxFactory.XmlText(parts[4]));
        }

        private static XmlElementSyntax MessageParameterComment(ParameterSyntax messageParameter)
        {
            return ParameterComment(messageParameter, Constants.Comments.ExceptionCtorMessageParamPhrase);
        }

        private static XmlElementSyntax ParameterComment(ParameterSyntax parameter, string[] comments)
        {
            return Comment(SyntaxFactory.XmlParamElement(parameter.Identifier.ValueText), comments[0]);
        }
    }
}