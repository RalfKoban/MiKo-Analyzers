using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2200_DocumentationStartsCapitalizedAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2200";

        private static readonly string[] XmlTags =
                                                   {
                                                       Constants.XmlTag.Example,
                                                       Constants.XmlTag.Exception,
                                                       Constants.XmlTag.Note,
                                                       Constants.XmlTag.Overloads,
                                                       Constants.XmlTag.Para,
                                                       Constants.XmlTag.Param,
                                                       Constants.XmlTag.Permission,
                                                       Constants.XmlTag.Remarks,
                                                       Constants.XmlTag.Returns,
                                                       Constants.XmlTag.Summary,
                                                       Constants.XmlTag.TypeParam,
                                                       Constants.XmlTag.Value,
                                                   };

        public MiKo_2200_DocumentationStartsCapitalizedAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var xml in comment.GetXmlSyntax(XmlTags))
            {
                if (xml.Content.FirstOrDefault() is XmlTextSyntax text)
                {
                    yield return AnalyzeText(text, xml.GetName());
                }
            }
        }

        private Diagnostic AnalyzeText(XmlTextSyntax syntax, string xmlTag)
        {
            foreach (var token in syntax.TextTokens.OfKind(SyntaxKind.XmlTextLiteralToken))
            {
                var text = token.ValueText.Without(Constants.Comments.SpecialOrPhrase);
                var trimmedText = text.TrimStart();

                if (trimmedText.Length > 0)
                {
                    if (trimmedText[0].IsUpperCase())
                    {
                        // break out of inner foreach as this is a correct upper case
                        return null;
                    }

                    // find the starting character, but ignore white-spaces
                    var start = token.SpanStart + (text.Length - trimmedText.Length);

                    // we want to underline only the first character
                    var end = start + 1;

                    var location = CreateLocation(token, start, end);

                    return Issue(location, xmlTag);
                }
            }

            return null;
        }
    }
}