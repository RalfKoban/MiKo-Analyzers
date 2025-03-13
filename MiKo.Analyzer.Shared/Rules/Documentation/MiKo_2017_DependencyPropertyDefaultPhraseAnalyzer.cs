using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2017_DependencyPropertyDefaultPhraseAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2017";

        public MiKo_2017_DependencyPropertyDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IFieldSymbol field && field.Type.IsDependencyProperty();

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.DependencyProperty.FieldSuffix, StringComparison.OrdinalIgnoreCase) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            var propertyName = symbolName.WithoutSuffix(Constants.DependencyProperty.FieldSuffix);

            var containingType = symbol.ContainingType;

            if (containingType.GetMembersIncludingInherited<IPropertySymbol>(propertyName).None())
            {
                // it's an unknown dependency property
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> issues = null;

            var commentXml = symbol.GetDocumentationCommentXml();
            var containingTypeFullName = containingType.ToString();

            // loop over phrases for summaries and values
            var summaries = CommentExtensions.GetSummaries(commentXml);

            if (summaries.Count != 0)
            {
                var summaryPhrases = Phrases(Constants.Comments.DependencyPropertyFieldSummaryPhrase, containingTypeFullName, propertyName);

                foreach (var summary in summaries)
                {
                    if (summaryPhrases.None(_ => summary.StartsWith(_, StringComparison.Ordinal)))
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(Issue(symbol, Constants.XmlTag.Summary, summaryPhrases[0]));
                    }
                }
            }

            var values = CommentExtensions.GetValue(commentXml);

            if (values.Count != 0)
            {
                var valuePhrases = Phrases(Constants.Comments.DependencyPropertyFieldValuePhrase, containingTypeFullName, propertyName);

                foreach (var value in values)
                {
                    if (valuePhrases.None(_ => value.StartsWith(_, StringComparison.Ordinal)))
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(Issue(symbol, Constants.XmlTag.Value, valuePhrases[0]));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        private static List<string> Phrases(string[] phrases, string typeName, string propertyName)
        {
            var propertyFullName = typeName + "." + propertyName;

            var results = new List<string>(2 * phrases.Length);
            results.AddRange(phrases.Select(_ => _.FormatWith(propertyName))); // output as message to user
            results.AddRange(phrases.Select(_ => _.FormatWith(propertyFullName)));

            return results;
        }
    }
}