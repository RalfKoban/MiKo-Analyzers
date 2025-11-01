using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2219_DocumentationContainsNoQuestionOrExclamationMarkAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2219";

        private static readonly string[] Terms = { "?", "!" };

        private static readonly HashSet<string> AllowedTags = new HashSet<string>
                                                                  {
                                                                      Constants.XmlTag.C,
                                                                      Constants.XmlTag.Code,
                                                                      Constants.XmlTag.Note,
                                                                  };

        public MiKo_2219_DocumentationContainsNoQuestionOrExclamationMarkAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var token in comment.DescendantNodes<XmlTextSyntax>(_ => _.AncestorsWithinDocumentation<XmlElementSyntax>().None(__ => AllowedTags.Contains(__.GetName())))
                                         .SelectMany(_ => _.TextTokens.OfKind(SyntaxKind.XmlTextLiteralToken)))
            {
                var locations = GetAllLocations(token, Terms);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    for (var index = 0; index < locationsCount; index++)
                    {
                        var location = locations[index];
                        var word = location.GetSurroundingWord();

                        if (word.IsHyperlink())
                        {
                            continue;
                        }

                        if (results is null)
                        {
                            results = new List<Diagnostic>(1);
                        }

                        results.Add(Issue(location));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}