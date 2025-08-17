using System;
using System.Collections.Generic;

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

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var xml in comment.GetEmptyXmlSyntax(Tags))
            {
                var tagName = xml.GetName();

                if (tagName is Constants.XmlTag.Code && xml.Attributes.Any(_ => _.GetName() is "source"))
                {
                    // ignore <code> tags with a 'source' attribute as that attribute refers to the coding snippet
                    continue;
                }

                if (results is null)
                {
                    results = new List<Diagnostic>(1);
                }

                results.Add(Issue(xml, tagName));
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}