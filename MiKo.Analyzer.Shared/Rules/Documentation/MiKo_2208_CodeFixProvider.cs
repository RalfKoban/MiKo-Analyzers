﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2208_CodeFixProvider)), Shared]
    public sealed class MiKo_2208_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = CreateReplacementMap();

        public override string FixableDiagnosticId => MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer.Id;

        protected override string Title => Resources.MiKo_2208_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            return syntax.ReplaceTokens(
                                        syntax.DescendantTokens(SyntaxKind.XmlTextLiteralToken).Where(_ => _.GetLocation().Contains(diagnostic.Location)),
                                        (original, rewritten) => original.WithText(ReplaceText(original.Text)));
        }

        private static StringBuilder ReplaceText(string originalText)
        {
            var result = new StringBuilder(originalText);

            foreach (var pair in ReplacementMap)
            {
                result.Replace(pair.Key, pair.Value);
            }

            return result;
        }

        private static Dictionary<string, string> CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var phrase in MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer.InstanceOfPhrase)
            {
                var upperCase = phrase[0].IsUpperCase();

                dictionary.Add(phrase + "a ", upperCase ? "A " : "a ");
                dictionary.Add(phrase + "an ", upperCase ? "An " : "an ");
                dictionary.Add(phrase, phrase.FirstWord() + " ");
            }

            return dictionary;
        }
    }
}