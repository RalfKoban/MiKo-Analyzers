using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2044_InvalidSeeParameterInXmlAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2044";

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                           {
                                                               Constants.XmlTag.See,
                                                               Constants.XmlTag.SeeAlso,
                                                           };

        public MiKo_2044_InvalidSeeParameterInXmlAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var method = (IMethodSymbol)symbol;
            var names = method.Parameters.ToHashSet(_ => _.Name);

            if (names.Count > 0)
            {
                foreach (var node in comment.DescendantNodes(_ => true, true))
                {
                    switch (node.Kind())
                    {
                        case SyntaxKind.XmlElement:
                        case SyntaxKind.XmlEmptyElement:
                        {
                            var tag = node.GetXmlTagName();

                            if (Tags.Contains(tag))
                            {
                                var cref = node.GetCref();

                                if (cref != null)
                                {
                                    var name = cref.GetCrefType().GetName();

                                    if (names.Contains(name))
                                    {
                                        yield return Issue(symbol.Name, node, node.GetText());
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}