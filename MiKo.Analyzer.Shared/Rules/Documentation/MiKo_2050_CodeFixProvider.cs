using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2050_CodeFixProvider)), Shared]
    public sealed class MiKo_2050_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
//// ncrunch: rdi off
        private static readonly Dictionary<string, string> TypeReplacementMap = CreateTypePhrases().Except(Constants.Comments.ExceptionTypeSummaryStartingPhrase).ToDictionary(_ => _, _ => string.Empty);
//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2050";

        protected override string Title => Resources.MiKo_2050_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var ctor = syntax.FirstAncestorOrSelf<ConstructorDeclarationSyntax>();

            if (ctor != null)
            {
                return FixCtorComment(ctor);
            }

            // we have only the type
            return FixTypeSummary(syntax);
        }

        private static DocumentationCommentTriviaSyntax FixTypeSummary(DocumentationCommentTriviaSyntax comment)
        {
            const string Phrase = Constants.Comments.ExceptionTypeSummaryStartingPhrase;

            var summary = comment.GetXmlSyntax(Constants.XmlTag.Summary).FirstOrDefault();

            if (summary is null)
            {
                var newSummary = Comment(SyntaxFactory.XmlSummaryElement(), Phrase).WithTrailingXmlComment();

                return comment.InsertNodeAfter(comment.Content[0], newSummary);
            }
            else
            {
                var preparedSummary = Comment(summary, TypeReplacementMap.Keys, TypeReplacementMap, FirstWordHandling.MakeLowerCase);
                var newSummary = CommentStartingWith(preparedSummary, Phrase);

                return comment.ReplaceNode(summary, newSummary);
            }
        }

        private static DocumentationCommentTriviaSyntax FixCtorComment(ConstructorDeclarationSyntax ctor)
        {
            var typeDeclarationSyntax = ctor.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            var type = SyntaxFactory.ParseTypeName(typeDeclarationSyntax?.Identifier.ValueText ?? string.Empty);

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

        private static DocumentationCommentTriviaSyntax FixParameterlessCtor(TypeSyntax type)
        {
            var parts = (Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate + ".").FormatWith('|').Split('|');

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), parts[0], SeeCref(type), parts[1]);

            return SyntaxFactory.DocumentationComment(summary.WithEndOfLine());
        }

        private static DocumentationCommentTriviaSyntax FixMessageParamCtor(TypeSyntax type, ParameterSyntax messageParameter)
        {
            const string Template = Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate + Constants.Comments.ExceptionCtorMessageParamSummaryContinuingPhrase + ".";

            var parts = Template.FormatWith('|').Split('|');

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), parts[0], SeeCref(type), parts[1]);
            var param = MessageParameterComment(messageParameter);

            return SyntaxFactory.DocumentationComment(
                                                  summary.WithTrailingXmlComment(),
                                                  param.WithEndOfLine());
        }

        private static DocumentationCommentTriviaSyntax FixMessageExceptionParamCtor(TypeSyntax type, ParameterSyntax messageParameter, ParameterSyntax exceptionParameter)
        {
            const string Template = Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate
                                  + Constants.Comments.ExceptionCtorMessageParamSummaryContinuingPhrase
                                  + Constants.Comments.ExceptionCtorExceptionParamSummaryContinuingPhrase
                                  + ".";

            var summaryParts = Template.FormatWith('|').Split('|');

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), summaryParts[0], SeeCref(type), summaryParts[1]);
            var param1 = MessageParameterComment(messageParameter);
            var param2 = ExceptionParameterComment(exceptionParameter);

            return SyntaxFactory.DocumentationComment(
                                                  summary.WithTrailingXmlComment(),
                                                  param1.WithTrailingXmlComment(),
                                                  param2.WithEndOfLine());
        }

        private static DocumentationCommentTriviaSyntax FixSerializationParamCtor(TypeSyntax type, ParameterSyntax serializationInfoParameter, ParameterSyntax streamingContextParameter)
        {
            const string Template = Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate
                                  + Constants.Comments.ExceptionCtorSerializationParamSummaryContinuingPhrase
                                  + ".";

            var summaryParts = Template.FormatWith('|').Split('|');

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
            var parameterName = exceptionParameter.GetName();

            var parts = Constants.Comments.ExceptionCtorExceptionParamPhraseTemplate.FormatWith('|', '|', '|', '|').Split('|');

            var catchBlock = XmlElement("b", XmlText("catch"));
            var paramRef = SyntaxFactory.XmlParamRefElement(parameterName);

            return Comment(
                       SyntaxFactory.XmlParamElement(parameterName),
                       XmlText(parts[0]),
                       Para().WithLeadingXmlComment().WithTrailingXmlComment(),
                       XmlText(parts[1]),
                       paramRef,
                       XmlText(parts[2]),
                       SeeLangword_Null(),
                       XmlText(parts[3]),
                       catchBlock,
                       XmlText(parts[4]));
        }

        private static XmlElementSyntax MessageParameterComment(ParameterSyntax messageParameter) => ParameterComment(messageParameter, Constants.Comments.ExceptionCtorMessageParamPhrase);

//// ncrunch: rdi off
        private static IEnumerable<string> CreateTypePhrases()
        {
            var starts = new[]
                             {
                                 "A exception", "An exception", "The exception", "This exception", "Exception",
                                 "A general exception", "An general exception", "The general exception", "This general exception", "General exception",
                                 "A most general exception", "An most general exception", "The most general exception", "This most general exception", "Most general exception",
                             };
            var verbs = new[] { "that is thrown", "which is thrown", "thrown", "to throw", "that is fired", "which is fired", "fired", "to fire" };
            var conditions = new[] { "if", "when", "in case" };

            foreach (var condition in conditions)
            {
                yield return "Fire " + condition;
                yield return "Fired " + condition;

                yield return "Indicates that " + condition;

                yield return "Occurs " + condition;

                yield return "Throw " + condition;
                yield return "Thrown " + condition;
            }

            foreach (var start in starts)
            {
                yield return start + " is used by ";
                yield return start + " that is used by ";
                yield return start + " which is used by ";
                yield return start + " used by ";
                yield return start + " indicates that ";
                yield return start + " that indicates that ";
                yield return start + " which indicates that ";
                yield return start + " indicating that ";

                foreach (var verb in verbs)
                {
                    var begin = string.Concat(start, " ", verb, " ");

                    foreach (var condition in conditions)
                    {
                        yield return string.Concat(begin, condition);
                    }
                }
            }
        }
//// ncrunch: rdi default
    }
}