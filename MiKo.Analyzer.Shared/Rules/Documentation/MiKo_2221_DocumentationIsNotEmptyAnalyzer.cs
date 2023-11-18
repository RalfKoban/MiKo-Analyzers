using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2221_DocumentationIsNotEmptyAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2221";

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                           {
                                                               Constants.XmlTag.Overloads,
                                                               Constants.XmlTag.Summary,
                                                               Constants.XmlTag.Remarks,
                                                               Constants.XmlTag.Returns,
                                                               Constants.XmlTag.Example,
                                                               Constants.XmlTag.Exception,
                                                               Constants.XmlTag.Code,
                                                               Constants.XmlTag.Note,
                                                               Constants.XmlTag.Value,
                                                           };

        public MiKo_2221_DocumentationIsNotEmptyAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var xml in comment.GetEmptyXmlSyntax(Tags))
            {
                var tagName = xml.GetName();

                if (tagName == Constants.XmlTag.Code && xml.Attributes.Any(_ => _.GetName() == "source"))
                {
                    // ignore <code> tags with a 'source' attribute as that attribute refers to the coding snippet
                    continue;
                }

                yield return Issue(symbol.Name, xml, tagName);
            }
        }
    }
}