using System;
using System.Linq;

using Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2230_ReturnValueUsesListAnalyzer : ReturnsValueDocumentationAnalyzer
    {
        public const string Id = "MiKo_2230";

        public MiKo_2230_ReturnValueUsesListAnalyzer() : base(Id)
        {
        }

        protected override Diagnostic[] AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag)
        {
            var xmlSyntax = comment.GetXmlSyntax(xmlTag);

            if (xmlSyntax.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return xmlSyntax.SelectMany(_ => _.Content.OfType<XmlTextSyntax>())
                            .Where(_ => _.GetTextTrimmed().Contains(Constants.Comments.ValueMeaningPhrase, StringComparison.Ordinal))
                            .Select(_ => Issue(_))
                            .ToArray();
        }
    }
}
