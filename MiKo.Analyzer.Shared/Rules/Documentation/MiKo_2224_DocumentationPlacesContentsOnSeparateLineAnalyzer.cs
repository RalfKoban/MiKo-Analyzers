using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2224";

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                           {
                                                               Constants.XmlTag.Overloads,
                                                               Constants.XmlTag.Note,
                                                               Constants.XmlTag.Summary,
                                                               Constants.XmlTag.Exception,
                                                               Constants.XmlTag.Param,
                                                               Constants.XmlTag.Para, // (except for <para>-or-</para>)
                                                               Constants.XmlTag.TypeParam,
                                                               Constants.XmlTag.Returns,
                                                               Constants.XmlTag.Value,
                                                               Constants.XmlTag.Remarks,
                                                               Constants.XmlTag.Example,
                                                               Constants.XmlTag.List,
                                                           };

        public MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(comment);

        private static bool IsOnSameLine(SyntaxList<XmlNodeSyntax> contents, ICollection<int> lines)
        {
            foreach (var content in contents)
            {
                if (content is XmlTextSyntax text)
                {
                    foreach (var token in text.TextTokens)
                    {
                        if (token.ValueText.IsNullOrWhiteSpace())
                        {
                            continue;
                        }

                        var line = token.GetLocation().GetLineSpan().StartLinePosition.Line;

                        if (lines.Contains(line))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    var line = content.GetLocation().GetLineSpan().StartLinePosition.Line;

                    if (lines.Contains(line))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsOnSameLine(SyntaxList<XmlNodeSyntax> contents, params int[] lines) => IsOnSameLine(contents, (ICollection<int>)lines);

        private IEnumerable<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment)
        {
            var lines = new HashSet<int>();

            foreach (var element in comment.DescendantNodes<XmlElementSyntax>(_ => Tags.Contains(_.GetName())))
            {
                if (element.GetName() == Constants.XmlTag.Para && element.GetTextTrimmed().Equals(Constants.Comments.SpecialOrPhrase.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    // that is allowed
                    continue;
                }

                var startTagLine = element.StartTag.GetLocation().GetLineSpan().StartLinePosition.Line;
                var endTagLine = element.EndTag.GetLocation().GetLineSpan().StartLinePosition.Line;

                var startLineOnSeparateLine = lines.Add(startTagLine);
                var endLineOnSeparateLine = lines.Add(endTagLine);

                if (startLineOnSeparateLine)
                {
                    if (IsOnSameLine(element.Content, startTagLine))
                    {
                        yield return Issue(element.StartTag);
                    }
                }
                else
                {
                    yield return Issue(element.StartTag);
                }

                if (endLineOnSeparateLine)
                {
                    if (IsOnSameLine(element.Content, endTagLine))
                    {
                        yield return Issue(element.EndTag);
                    }
                }
                else
                {
                    yield return Issue(element.EndTag);
                }
            }
        }
    }
}