﻿using System;
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
        private static readonly string[] TypeReplacementMapKeys = CreateTypePhrases().Except(Constants.Comments.ExceptionTypeSummaryStartingPhrase).ToArray();

        private static readonly Pair[] TypeReplacementMap = TypeReplacementMapKeys.ToArray(_ => new Pair(_));
//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2050";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic issue)
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

            var summaries = comment.GetXmlSyntax(Constants.XmlTag.Summary);

            if (summaries.Count is 0)
            {
                var newSummary = Comment(SyntaxFactory.XmlSummaryElement(), Phrase).WithTrailingXmlComment();

                return comment.InsertNodeAfter(comment.Content[0], newSummary);
            }
            else
            {
                var summary = summaries[0];
                var preparedSummary = Comment(summary, TypeReplacementMapKeys, TypeReplacementMap, FirstWordAdjustment.StartLowerCase);
                var newSummary = CommentStartingWith(preparedSummary, Phrase);

                return comment.ReplaceNode(summary, newSummary);
            }
        }

        private static DocumentationCommentTriviaSyntax FixCtorComment(ConstructorDeclarationSyntax ctor)
        {
            var typeDeclarationSyntax = ctor.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            var type = (typeDeclarationSyntax?.Identifier.ValueText ?? string.Empty).AsTypeSyntax();

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
            var parts = (Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate + ".").FormatWith("|").Split('|');

            var summary = Comment(SyntaxFactory.XmlSummaryElement(), parts[0], SeeCref(type), parts[1]);

            return SyntaxFactory.DocumentationComment(summary.WithEndOfLine());
        }

        private static DocumentationCommentTriviaSyntax FixMessageParamCtor(TypeSyntax type, ParameterSyntax messageParameter)
        {
            const string Template = Constants.Comments.ExceptionCtorSummaryStartingPhraseTemplate + Constants.Comments.ExceptionCtorMessageParamSummaryContinuingPhrase + ".";

            var parts = Template.FormatWith("|").Split('|');

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

            var summaryParts = Template.FormatWith("|").Split('|');

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

            var summaryParts = Template.FormatWith("|").Split('|');

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

            var parts = Constants.Comments.ExceptionCtorExceptionParamPhraseTemplate.FormatWith("|", "|", "|", "|").Split('|');

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
        private static HashSet<string> CreateTypePhrases()
        {
            var starts = new[]
                             {
                                 "A exception", "An exception", "The exception", "This exception", "Exception",
                                 "A general exception", "An general exception", "The general exception", "This general exception", "General exception",
                                 "A most general exception", "An most general exception", "The most general exception", "This most general exception", "Most general exception",
                             };
            var verbs = new[] { "that is thrown", "which is thrown", "is thrown", "thrown", "to throw", "that is fired", "which is fired", "fired", "to fire" };
            var conditions = new[] { "if", "when", "in case" };

            var results = new HashSet<string>();

            foreach (var condition in conditions)
            {
                results.Add("Fire " + condition);
                results.Add("Fired " + condition);
                results.Add("Indicates that " + condition);
                results.Add("Occurs " + condition);
                results.Add("Throw " + condition);
                results.Add("Thrown " + condition);
            }

            foreach (var start in starts)
            {
                results.Add(start + " is used by ");
                results.Add(start + " that is used by ");
                results.Add(start + " which is used by ");
                results.Add(start + " used by ");
                results.Add(start + " indicates that ");
                results.Add(start + " that indicates that ");
                results.Add(start + " which indicates that ");
                results.Add(start + " indicating that ");

                foreach (var verb in verbs)
                {
                    var middle = string.Concat(" ", verb, " ");

                    var begin = string.Concat(start, middle);
                    var beginLowerCase = begin.ToLowerCaseAt(0);

                    foreach (var condition in conditions)
                    {
                        results.Add(string.Concat(begin, condition));
                        results.Add(string.Concat("Represent ", beginLowerCase, condition));
                        results.Add(string.Concat("Represents ", beginLowerCase, condition));
                    }
                }
            }

            return results;
        }
//// ncrunch: rdi default
    }
}