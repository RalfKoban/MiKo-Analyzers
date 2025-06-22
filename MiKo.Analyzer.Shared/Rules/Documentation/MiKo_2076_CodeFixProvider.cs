﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2076_CodeFixProvider)), Shared]
    public sealed class MiKo_2076_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2076";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlElementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax element)
            {
                XmlTextSyntax startText;
                XmlTextSyntax endText;

                if (element.StartTag.IsOnSameLineAs(element.EndTag))
                {
                    startText = XmlText(" " + Constants.Comments.DefaultStartingPhrase);

                    // no trailing '///' to add because the text is located on the same line
                    endText = XmlText(".");
                }
                else
                {
                    startText = XmlText(Constants.Comments.DefaultStartingPhrase);

                    endText = XmlText(".").WithTrailingXmlComment();
                }

                var reference = GetDefaultValueReference(issue);

                return element.AddContent(startText, reference, endText);
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }

        private static XmlNodeSyntax GetDefaultValueReference(Diagnostic issue)
        {
            if (issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.DefaultSeeLangwordValue, out var defaultValue))
            {
                return SeeLangword(defaultValue);
            }

            if (issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, out var defaultCrefValue))
            {
                return SeeCref(defaultCrefValue);
            }

            if (issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, out var defaultCodeValue))
            {
                return C(defaultCodeValue);
            }

            return XmlText(Constants.TODO);
        }
    }
}