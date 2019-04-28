using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            foreach (var xmlTag in XmlTags)
            {
                foreach (var _ in CommentExtensions.GetCommentElements(commentXml, xmlTag)
                                     .Select(_ => _.Nodes().ConcatenatedWith().TrimStart())
                                     .Select(_ => _.Remove(Constants.Comments.SpecialOrPhrase))
                                     .Where(_ => _.Length > 0)
                                     .Where(_ => !_[0].IsUpperCase() && _[0] != Constants.Comments.XmlElementStartingTag[0]))
                {
                    yield return Issue(symbol, xmlTag);
                }
            }
        }
    }
}