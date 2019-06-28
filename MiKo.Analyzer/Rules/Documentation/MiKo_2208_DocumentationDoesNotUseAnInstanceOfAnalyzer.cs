using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2208";

        public MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => Constants.Comments.InstanceOfPhrase
                                                                                                                 .Where(_ => commentXml.Contains(_, StringComparison.Ordinal))
                                                                                                                 .Select(_ => Issue(symbol, _));
    }
}