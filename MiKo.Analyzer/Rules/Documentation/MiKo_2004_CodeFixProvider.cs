using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2004_CodeFixProvider)), Shared]
    public sealed class MiKo_2004_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2004_EventHandlerParametersAnalyzer.Id;

        protected override string Title => Resources.MiKo_2004_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax Comment(Document document, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic)
        {
            // comment is missing, so add one
            var method = comment.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (method != null)
            {
                // find parameter
                var isSender = diagnostic.Properties.ContainsKey(MiKo_2004_EventHandlerParametersAnalyzer.IsSender);
                var index = isSender ? 0 : 1;
                var parameter = method.ParameterList.Parameters[index];

                var paramElement = ParamComment(parameter.GetName());
                var content = Comment(document, paramElement, parameter, index).WithLeadingXmlCommentExterior();

                // find summary to add sender
                if (isSender)
                {
                    var summary = comment.GetXmlSyntax(Constants.XmlTag.Summary).FirstOrDefault();
                    if (summary != null)
                    {
                        comment = comment.InsertNodeAfter(summary, content);

                        // fix situation that content got placed on same line as summary end tag
                        summary = comment.GetXmlSyntax(Constants.XmlTag.Summary).First();

                        return comment.ReplaceNode(summary, summary.WithTrailingNewLine());
                    }
                }
                else
                {
                    // we are event args and have to fix the situation that the method gets suddenly placed on comment line
                    content = content.WithTrailingNewLine();
                }

                return comment.AddContent(content);
            }

            return comment;
        }

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            if (index == 0)
            {
                // this is the sender
                return Comment(comment, Constants.Comments.EventSourcePhrase);
            }

            // this is the event args
            var startingPhrase = MiKo_2004_EventHandlerParametersAnalyzer.GetEventArgsStartingPhrase(parameter.Type.GetNameOnlyPart());
            var endingPhrase = MiKo_2004_EventHandlerParametersAnalyzer.GetEventArgsEndingPhrase() + ".";

            return Comment(comment, startingPhrase, parameter.Type, endingPhrase);
        }

        private static XmlElementSyntax ParamComment(string parameterName)
        {
            var tagName = SyntaxFactory.XmlName(Constants.XmlTag.Param);
            var startTag = SyntaxFactory.XmlElementStartTag(tagName, new SyntaxList<XmlAttributeSyntax>(SyntaxFactory.XmlNameAttribute(parameterName)));
            var endTag = SyntaxFactory.XmlElementEndTag(tagName);

            return SyntaxFactory.XmlElement(startTag, endTag);
        }
    }
}