﻿using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2019_CodeFixProvider)), Shared]
    public sealed class MiKo_2019_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string[] RepresentsCandidates =
                                                                {
                                                                    "Repository ",
                                                                };

        private static readonly Pair[] ReplacementMap =
                                                        {
                                                            new Pair("Called to "),
                                                        };

        private static readonly string[] ReplacementMapKeys = ReplacementMap.ToArray(_ => _.Key);

        public override string FixableDiagnosticId => "MiKo_2019";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax summary)
            {
                var content = summary.Content;

                if (content.Count > 0 && content[0] is XmlTextSyntax textSyntax)
                {
                    var startText = textSyntax.GetTextWithoutTrivia();

                    if (startText.StartsWithAny(RepresentsCandidates, StringComparison.Ordinal))
                    {
                        return CommentStartingWith(summary, "Represents a ");
                    }

                    var updatedSummary = MiKo_2012_CodeFixProvider.GetUpdatedSyntax(summary, textSyntax);

                    if (ReferenceEquals(summary, updatedSummary))
                    {
                        // nothing updated, so update by ourselves
                        return Comment(summary, ReplacementMapKeys, ReplacementMap, FirstWordHandling.MakeUpperCase | FirstWordHandling.MakeThirdPersonSingular | FirstWordHandling.KeepLeadingSpace);
                    }

                    return updatedSummary;
                }
            }

            return syntax;
        }
    }
}