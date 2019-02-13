﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules
{
    [TestFixture]
    public static class AnalyzerTests
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager(typeof(Resources));
        private static readonly List<Analyzer> AllAnalyzers = CreateAllAnalyzers();

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void Resources_contains_texts(Analyzer analyzer)
        {
            var findings = new[]
                               {
                                   analyzer.DiagnosticId + "_Description",
                                   analyzer.DiagnosticId + "_MessageFormat",
                                   analyzer.DiagnosticId + "_Title",
                               }
                           .Where(name => string.IsNullOrWhiteSpace(ResourceManager.GetString(name)))
                           .ToList();

            findings.Sort();

            Assert.That(findings, Is.Empty);
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void Titles_should_end_with_dot(Analyzer analyzer) => Assert.That(ResourceManager.GetString(analyzer.DiagnosticId + "_Title"), Does.EndWith("."));

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void Descriptions_should_end_with_dot(Analyzer analyzer) => Assert.That(ResourceManager.GetString(analyzer.DiagnosticId + "_Description"), Does.EndWith(".").Or.EndsWith(")"));

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void Messages_should_not_end_with_dot(Analyzer analyzer) => Assert.That(ResourceManager.GetString(analyzer.DiagnosticId + "_MessageFormat"), Does.Not.EndWith("."));

        [Test, Combinatorial, Ignore("Just to check from time to time whether the texts are acceptable or need to be rephrased.")]
        public static void Messages_should_not_contain_(
            [ValueSource(nameof(AllAnalyzers))] Analyzer analyzer,
            [Values("shall", "should")] string word)
            => Assert.That(ResourceManager.GetString(analyzer.DiagnosticId + "_MessageFormat"), Does.Not.Contain(word));

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void Analyzers_start_with_their_Id(Analyzer analyzer) => Assert.That(analyzer.GetType().Name, Is.Not.Null.And.StartsWith(analyzer.DiagnosticId + "_"));

        [Test]
        public static void Analyzers_have_unique_Ids()
        {
            var findings = new List<string>();

            var ids = new Dictionary<string, string>();
            foreach (var analyzer in AllAnalyzers)
            {
                var name = analyzer.DiagnosticId + ": " + analyzer.GetType().Name;

                if (ids.ContainsKey(analyzer.DiagnosticId))
                    findings.Add(name);
                else
                    ids[analyzer.DiagnosticId] = name;
            }

            findings.Sort();

            Assert.That(findings, Is.Empty);
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void Analyzer_starts_with_correct_number(Analyzer analyzer)
        {
            var id = GetDiagnosticIdStartingNumber(analyzer);

            Assert.That(analyzer.DiagnosticId, Is.Not.Null.And.StartsWith("MiKo_" + id));
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public static void Analyzer_are_marked_with_DiagnosticAnalyzer_attribute(Analyzer analyzer)
        {
            Assert.That(analyzer.GetType(), Has.Attribute<DiagnosticAnalyzerAttribute>().With.Property(nameof(DiagnosticAnalyzerAttribute.Languages)).EquivalentTo(new[] { LanguageNames.CSharp }));
        }

        [Test, Explicit("Test shall be run explicitly as it generates some markdown for the README.md file"), Ignore("Disabled")]
        public static void Analyzer_documentation_for_Markdown()
        {
            var markdownBuilder = new StringBuilder().AppendLine()
                                                     .AppendLine("## Available Rules")
                                                     .AppendLine("The following tables list all the rules that are currently provided by the analyzer.");

            var category = string.Empty;
            var tableFormat = "|{0}|{1}|{2}|" + Environment.NewLine;

            foreach (var descriptor in AllAnalyzers.Select(_ => _.GetType().GetProperty("Rule", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_)).OfType<DiagnosticDescriptor>().OrderBy(_ => _.Id).ThenBy(_ => _.Category))
            {
                if (category != descriptor.Category)
                {
                    category = descriptor.Category;

                    markdownBuilder
                        .AppendLine()
                        .AppendLine("### " + category)
                        .AppendFormat(tableFormat, "ID", "Title", "Enabled by default")
                        .AppendFormat(tableFormat, ":-", ":----", ":----------------:");
                }

                markdownBuilder.AppendFormat(tableFormat,
                                             descriptor.Id,
                                             descriptor.Title.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"),
                                             descriptor.IsEnabledByDefault ? ":white_check_mark:" : ":white_large_square:");
            }

            var markdown = markdownBuilder.ToString();

            File.WriteAllText(@"z:\test.md", markdown);

            Assert.That(markdown, Is.Empty);
        }

        private static int GetDiagnosticIdStartingNumber(Analyzer analyzer)
        {
            switch (analyzer.GetType().Namespace)
            {
                case "MiKoSolutions.Analyzers.Rules.Documentation": return 2;
                case "MiKoSolutions.Analyzers.Rules.Maintainability": return 3;
                case "MiKoSolutions.Analyzers.Rules.Metrics": return 0;
                case "MiKoSolutions.Analyzers.Rules.Naming": return 1;
                default: return -1;
            }
        }

        private static List<Analyzer> CreateAllAnalyzers()
        {
            var analyzerBaseType = typeof(Analyzer);

            var allAnalyzers = analyzerBaseType.Assembly.GetExportedTypes()
                                               .Where(_ => !_.IsAbstract && analyzerBaseType.IsAssignableFrom(_))
                                               .Select(_ => (Analyzer)_.GetConstructor(Type.EmptyTypes).Invoke(null))
                                               .ToList();
            return allAnalyzers;
        }
    }
}