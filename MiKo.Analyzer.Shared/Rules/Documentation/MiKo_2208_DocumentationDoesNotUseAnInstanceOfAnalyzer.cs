using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2208";

        internal static readonly string[] InstanceOfPhrase =
                                                             {
                                                                 "An instance of ",
                                                                 "an instance of ",
                                                                 "A instance of ",
                                                                 "a instance of ",
                                                                 "The instance of ",
                                                                 "the instance of ",
                                                                 "An object of ",
                                                                 "an object of ",
                                                                 "A object of ",
                                                                 "a object of ",
                                                                 "The object of ",
                                                                 "the object of ",
                                                                 "An instance if ",
                                                                 "an instance if ",
                                                                 "A instance if ",
                                                                 "a instance if ",
                                                                 "The instance if ",
                                                                 "the instance if ",
                                                             };

        public MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                const int EndOffset = 1; // we do not want to underline the last char

                foreach (var location in GetAllLocations(token, InstanceOfPhrase, StringComparison.Ordinal, 0, EndOffset))
                {
                    yield return Issue(location);
                }
            }
        }
    }
}