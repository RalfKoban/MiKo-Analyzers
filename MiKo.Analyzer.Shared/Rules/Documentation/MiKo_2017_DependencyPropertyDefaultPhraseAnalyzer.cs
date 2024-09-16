using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2017_DependencyPropertyDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2017";

        public MiKo_2017_DependencyPropertyDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.DependencyProperty.FieldSuffix, StringComparison.OrdinalIgnoreCase))
            {
                var propertyName = symbolName.WithoutSuffix(Constants.DependencyProperty.FieldSuffix);

                var containingType = symbol.ContainingType;

                if (containingType.GetMembersIncludingInherited<IPropertySymbol>(propertyName).Any())
                {
                    var containingTypeFullName = containingType.ToString();

                    // loop over phrases for summaries and values
                    var summaries = CommentExtensions.GetSummaries(commentXml);

                    if (summaries.Any())
                    {
                        var summaryPhrases = Phrases(Constants.Comments.DependencyPropertyFieldSummaryPhrase, containingTypeFullName, propertyName);

                        foreach (var summary in summaries)
                        {
                            if (summaryPhrases.None(_ => summary.StartsWith(_, StringComparison.Ordinal)))
                            {
                                yield return Issue(symbol, Constants.XmlTag.Summary, summaryPhrases[0]);
                            }
                        }
                    }

                    var values = CommentExtensions.GetValue(commentXml);

                    if (values.Any())
                    {
                        var valuePhrases = Phrases(Constants.Comments.DependencyPropertyFieldValuePhrase, containingTypeFullName, propertyName);

                        foreach (var value in values)
                        {
                            if (valuePhrases.None(_ => value.StartsWith(_, StringComparison.Ordinal)))
                            {
                                yield return Issue(symbol, Constants.XmlTag.Value, valuePhrases[0]);
                            }
                        }
                    }
                }
                else
                {
                    // it's an unknown dependency property
                }
            }
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