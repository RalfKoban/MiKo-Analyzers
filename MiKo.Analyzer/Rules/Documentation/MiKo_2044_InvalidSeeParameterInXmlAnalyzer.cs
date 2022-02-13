using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2044_InvalidSeeParameterInXmlAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2044";

        public MiKo_2044_InvalidSeeParameterInXmlAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            var method = (IMethodSymbol)symbol;
            var parameters = method.Parameters;

            if (parameters.Length > 0)
            {
                var crefs = Enumerable.Empty<CrefSyntax>()
                                      .Concat(CRefs(comment, Constants.XmlTag.See))
                                      .Concat(CRefs(comment, Constants.XmlTag.SeeAlso));

                foreach (var cref in crefs)
                {
                    foreach (var parameter in parameters)
                    {
                        if (cref.ToString() == parameter.Name)
                        {
                            // TODO RKN Add phrases
                            yield return Issue(parameter.Name, cref);
                        }
                    }
                }
            }
        }

        private static IEnumerable<CrefSyntax> CRefs(DocumentationCommentTriviaSyntax comment, string tagName)
        {
            var crefs = Enumerable.Empty<XmlCrefAttributeSyntax>()
                                  .Concat(comment.GetXmlSyntax(tagName).SelectMany(_ => _.GetAttributes<XmlCrefAttributeSyntax>()))
                                  .Concat(comment.GetEmptyXmlSyntax(tagName).SelectMany(_ => _.GetAttributes<XmlCrefAttributeSyntax>()))
                                  .Select(_ => _.Cref);

            return crefs;
        }
    }
}