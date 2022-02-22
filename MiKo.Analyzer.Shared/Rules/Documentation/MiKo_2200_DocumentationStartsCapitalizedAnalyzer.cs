using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var xmlTag in XmlTags)
            {
                foreach (var node in comment.GetXmlSyntax(xmlTag))
                {
                    foreach (var token in node.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
                    {
                        var content = token.ValueText.TrimStart();

                        if (content.Length > 0)
                        {
                            if (content == Constants.Comments.SpecialOrPhrase)
                            {
                                continue;
                            }

                            if (content[0].IsUpperCase() is false)
                            {
                                var location = GetFirstLocation(token, content[0].ToString());
                                yield return Issue(symbol.Name, location, xmlTag);
                            }
                        }
                    }
                }
            }
        }
    }
}