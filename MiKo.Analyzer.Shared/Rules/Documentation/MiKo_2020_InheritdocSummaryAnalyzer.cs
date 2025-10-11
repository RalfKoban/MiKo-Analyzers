using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2020_InheritdocSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2020";

        private static readonly string[] InheritMarkerTexts = { "See", "Impl", "Default" };

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                           {
                                                               Constants.XmlTag.See,
                                                               "SEE",
                                                               Constants.XmlTag.SeeAlso,
                                                               "SEEALSO",
                                                           };

        public MiKo_2020_InheritdocSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                case SymbolKind.Method:
                case SymbolKind.Property:
                case SymbolKind.Event:
                    return true;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> issues = null;

            foreach (var xmlTag in comment.GetSummaryXmls(Tags))
            {
                var cref = xmlTag.GetCref();

                if (cref is null)
                {
                    continue;
                }

                if (xmlTag.Parent is XmlElementSyntax e)
                {
                    var content = e.Content;

                    var index = content.IndexOf(xmlTag);

                    if (index is 0)
                    {
                        // is it the first inside the comment
                        if (HasIssue(symbol, semanticModel, cref))
                        {
                            if (issues is null)
                            {
                                issues = new List<Diagnostic>(1);
                            }

                            issues.Add(Issue(xmlTag));
                        }
                    }
                    else if (index > 0 && content[index - 1] is XmlTextSyntax t) // there might be multiple <see/> in a comment, so consider all of them
                    {
                        // we seem to have an issue here, so inspect the code
                        if (HasIssue(symbol, semanticModel, cref))
                        {
                            var text = t.GetTextTrimmed();

                            if (text.IsNullOrWhiteSpace())
                            {
                                if (issues is null)
                                {
                                    issues = new List<Diagnostic>(1);
                                }

                                issues.Add(Issue(xmlTag));
                            }
                            else
                            {
                                // inspect first and last word
                                var words = text.AsSpan().WordsAsSpan();

                                if (words.First().Text.StartsWithAny(InheritMarkerTexts, StringComparison.OrdinalIgnoreCase)
                                 || words.Last().Text.ContainsAny(InheritMarkerTexts, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (issues is null)
                                    {
                                        issues = new List<Diagnostic>(1);
                                    }

                                    issues.Add(Issue(xmlTag));
                                }
                            }
                        }
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        private static bool HasIssue(ISymbol symbol, SemanticModel semanticModel, XmlCrefAttributeSyntax cref)
        {
            if (symbol.IsOverride)
            {
                return true;
            }

            var type = cref.GetCrefType();
            var linkedSymbol = type?.GetSymbol(semanticModel);

            switch (symbol)
            {
                case IPropertySymbol _:
                {
                    return linkedSymbol is IPropertySymbol linked && linked.Name == symbol.Name && symbol.IsInterfaceImplementation();
                }

                case IEventSymbol _:
                {
                    return linkedSymbol is IEventSymbol linked && linked.Name == symbol.Name && symbol.IsInterfaceImplementation();
                }

                case IMethodSymbol _:
                {
                    return linkedSymbol is IMethodSymbol linked && linked.Name == symbol.Name && symbol.IsInterfaceImplementation();
                }

                case ITypeSymbol typeSymbol:
                {
                    return linkedSymbol is ITypeSymbol linked && typeSymbol.IsRelated(linked);
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}