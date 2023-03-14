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

        private static readonly IEnumerable<string> Tags = new HashSet<string>
                                                               {
                                                                   Constants.XmlTag.Overloads,
                                                                   Constants.XmlTag.Summary,
                                                                   Constants.XmlTag.Remarks,
                                                                   Constants.XmlTag.Returns,
                                                                   Constants.XmlTag.Example,
                                                                   Constants.XmlTag.Exception,
                                                                   Constants.XmlTag.Code,
                                                               };

        public MiKo_2221_DocumentationIsNotEmptyAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            return comment.GetEmptyXmlSyntax(Tags).Select(xml => Issue(symbol.Name, xml, xml.GetName()));
        }
    }
}