using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2100_ExampleDefaultPhraseAnalyzer : ExampleDocumentationAnalyzer
    {
        public const string Id = "MiKo_2100";

        public MiKo_2100_ExampleDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeExamples(ISymbol owningSymbol, IEnumerable<XmlElementSyntax> examples)
        {
            foreach (var example in examples)
            {
                yield return AnalyzeStart(owningSymbol, Constants.XmlTag.Example, example);
            }
        }

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            if (textToken.ValueText.StartsWith(Constants.Comments.ExampleDefaultPhrase))
            {
                return null;
            }

            return Issue(symbol, Constants.Comments.ExampleDefaultPhrase);
        }
    }
}