using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2004_CodeFixProvider)), Shared]
    public class MiKo_2004_CodeFixProvider : DocumentationCodeFixProvider
    {
        public sealed override string FixableDiagnosticId => MiKo_2004_EventHandlerParametersAnalyzer.Id;

        protected sealed override string Title => "Fix description of event handler parameter";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            var parameterName = syntaxNodes.OfType<ParameterSyntax>().First().Identifier.Text;

            // we are called for each parameter, s we have to find out the correct XML element
            return GetXmlSyntax(syntaxNodes.OfType<MethodDeclarationSyntax>()).FirstOrDefault(_ => GetParameterName(_) == parameterName);
        }

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var element = (XmlElementSyntax)syntax;
            var parameterName = GetParameterName(element);

            var method = syntax.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var parameters = method.ParameterList.Parameters;

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                if (parameter.GetName() == parameterName)
                {
                    if (index == 0)
                    {
                        // this is the sender
                        return element.WithContent(new SyntaxList<XmlNodeSyntax>(SyntaxFactory.XmlText(Constants.Comments.EventSourcePhrase[0])));
                    }

                    // this is the event args
                    var name = parameter.Type.GetNameOnlyPart();

                    var x = SyntaxFactory.List<XmlNodeSyntax>()
                                         .Add(SyntaxFactory.XmlText(name.StartsWithAnyChar("AEIOU") ? "An " : "A "))
                                         .Add(Cref(Constants.XmlTag.See, parameter.Type))
                                         .Add(SyntaxFactory.XmlText(" that contains the event data."));

                    return element.WithContent(x);
                }
            }

            return syntax;
        }

        private static XmlEmptyElementSyntax Cref(string name, TypeSyntax type)
        {
            var attribute = SyntaxFactory.XmlCrefAttribute(SyntaxFactory.TypeCref(type.WithoutTrailingTrivia()));

            return SyntaxFactory.XmlEmptyElement(name)
                                .WithAttributes(new SyntaxList<XmlAttributeSyntax>(attribute));
        }

        private static string GetParameterName(XmlElementSyntax syntax) => syntax.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().First().Identifier.GetName();
    }
}