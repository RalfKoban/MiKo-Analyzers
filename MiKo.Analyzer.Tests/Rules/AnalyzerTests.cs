﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Rules.Documentation;
using MiKoSolutions.Analyzers.Rules.Maintainability;
using MiKoSolutions.Analyzers.Rules.Naming;

using NCrunch.Framework;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules
{
    [TestFixture, Atomic, Isolated]
    public static class AnalyzerTests
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager(typeof(Resources));
        private static readonly Analyzer[] AllAnalyzers = CreateAllAnalyzers();
        private static readonly CodeFixProvider[] AllCodeFixProviders = CreateAllCodeFixProviders();

        [Test]
        public static void Resources_contains_texts_([ValueSource(nameof(AllAnalyzers))] Analyzer analyzer)
        {
            var findings = new[]
                               {
                                   analyzer.DiagnosticId + "_Description",
                                   analyzer.DiagnosticId + "_MessageFormat",
                                   analyzer.DiagnosticId + "_Title",
                               }
                           .Where(_ => string.IsNullOrWhiteSpace(ResourceManager.GetString(_)))
                           .ToList();

            findings.Sort();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        public static void Titles_should_end_with_dot_([ValueSource(nameof(AllAnalyzers))] Analyzer analyzer)
        {
            var key = analyzer.DiagnosticId + "_Title";

            Assert.That(ResourceManager.GetString(key), Does.EndWith("."), $"'{key}' is incorrect.{Environment.NewLine}");
        }

        [Test]
        public static void Descriptions_should_end_with_dot_([ValueSource(nameof(AllAnalyzers))] Analyzer analyzer)
        {
            var key = analyzer.DiagnosticId + "_Description";

            Assert.That(ResourceManager.GetString(key), Does.EndWith(".").Or.EndsWith(")"), $"'{key}' is incorrect.{Environment.NewLine}");
        }

        [Test]
        public static void Messages_should_not_end_with_dot_([ValueSource(nameof(AllAnalyzers))] Analyzer analyzer)
        {
            var key = analyzer.DiagnosticId + "_MessageFormat";

            Assert.That(ResourceManager.GetString(key), Does.Not.EndWith("."), $"'{key}' is incorrect.{Environment.NewLine}");
        }

        [Test, Combinatorial, Ignore("Just to check from time to time whether the texts are acceptable or need to be rephrased.")]
        public static void Messages_should_not_contain_(
                                                    [ValueSource(nameof(AllAnalyzers))] Analyzer analyzer,
                                                    [Values("shall", "should")] string word)
            => Assert.That(ResourceManager.GetString(analyzer.DiagnosticId + "_MessageFormat"), Does.Not.Contain(word));

        [Test]
        public static void Analyzers_start_with_their_Id_([ValueSource(nameof(AllAnalyzers))] Analyzer analyzer) => Assert.That(analyzer.GetType().Name, Is.Not.Null.And.StartsWith(analyzer.DiagnosticId + "_"));

        [Test]
        public static void Analyzers_have_unique_Ids()
        {
            var findings = new List<string>();

            var ids = new Dictionary<string, string>();
            foreach (var analyzer in AllAnalyzers)
            {
                var name = analyzer.DiagnosticId + ": " + analyzer.GetType().Name;

                if (ids.ContainsKey(analyzer.DiagnosticId))
                {
                    findings.Add(name);
                }
                else
                {
                    ids[analyzer.DiagnosticId] = name;
                }
            }

            findings.Sort();

            Assert.That(findings, Is.Empty);
        }

        [Test]
        public static void Analyzer_starts_with_correct_number_([ValueSource(nameof(AllAnalyzers))] Analyzer analyzer)
        {
            var id = GetDiagnosticIdStartingNumber(analyzer);

            Assert.That(analyzer.DiagnosticId, Is.Not.Null.And.StartsWith("MiKo_" + id));
        }

        [Test]
        public static void Analyzer_are_marked_with_DiagnosticAnalyzer_attribute_([ValueSource(nameof(AllAnalyzers))] Analyzer analyzer)
        {
            Assert.That(analyzer.GetType(), Has.Attribute<DiagnosticAnalyzerAttribute>().With.Property(nameof(DiagnosticAnalyzerAttribute.Languages)).EquivalentTo(new[] { LanguageNames.CSharp }));
        }

        [Test]
        public static void CodeFixProvider_are_marked_with_ExportCodeFixProvider_attribute_([ValueSource(nameof(AllCodeFixProviders))] CodeFixProvider provider)
        {
            var type = provider.GetType();

            Assert.That(type, Has.Attribute<ExportCodeFixProviderAttribute>().With.Property(nameof(ExportCodeFixProviderAttribute.Name)).EqualTo(type.Name));
        }

        [Test]
        public static void CodeFixProviders_use_Id_of_corresponding_Analyzer()
        {
            var map = AllAnalyzers.ToDictionary(_ => _.DiagnosticId);

            Assert.Multiple(() =>
                                {
                                    foreach (var id in AllCodeFixProviders.Select(_ => _.FixableDiagnosticIds.First()))
                                    {
                                        Assert.That(map.ContainsKey(id), Is.True, $"Analyzer for '{id}' missing");
                                    }
                                });
        }

        [Test]
        public static void CodeFixProvider_uses_simplified_name_([ValueSource(nameof(AllCodeFixProviders))] CodeFixProvider provider)
        {
            var id = provider.FixableDiagnosticIds.First();

            Assert.That(provider.GetType().Name, Does.StartWith(id).And.EndsWith("_CodeFixProvider"));
        }

        [Test]
        public static void CodeFixProvider_has_a_title_([ValueSource(nameof(AllCodeFixProviders))] CodeFixProvider provider)
        {
            var id = provider.FixableDiagnosticIds.First();

            var expectedTitle = Resources.ResourceManager.GetString(id + "_CodeFixTitle");

            var codeFixTitle = provider.GetType().GetProperty("Title", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(provider).ToString();

            var parts = string.Format(expectedTitle, '|').Split('|');
            if (parts.Length <= 1)
            {
                Assert.That(codeFixTitle, Is.EqualTo(expectedTitle));
            }
            else
            {
                Assert.That(codeFixTitle, Is.Not.EqualTo(expectedTitle), "Missing phrase");
                Assert.That(codeFixTitle, Does.StartWith(parts[0]).And.EndWith(parts[1]));
            }
        }

        [Test, Ignore("Just to find gaps")]
        public static void Gaps_in_Analyzer_numbers_([Range(1, 5, 1)] int i)
        {
            var gaps = new List<string>();

            var diagnosticIds = AllAnalyzers.Select(_ => _.DiagnosticId).ToArray();

            var thousand = i * 1000;

            for (var j = 0; j < thousand; j++)
            {
                var number = thousand + j;
                var prefix = $"MiKo_{number:####}";

                if (diagnosticIds.All(_ => _.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) is false))
                {
                    gaps.Add(prefix);
                }
            }

            Assert.That(gaps, Is.Empty, string.Join(Environment.NewLine, gaps));
        }

        [Test, Explicit("Test shall be run explicitly as it generates some markdown for the README.md file"), Ignore("Disabled")]
        public static void Analyzer_documentation_for_Markdown()
        {
            var markdownBuilder = new StringBuilder().AppendLine()
                                                     .AppendLine("## Available Rules")
                                                     .AppendLine($"The following tables list all the {AllAnalyzers.Length} rules that are currently provided by the analyzer.");

            var category = string.Empty;
            var tableFormat = "|{0}|{1}|{2}|{3}|" + Environment.NewLine;

            var codeFixProviders = AllCodeFixProviders.SelectMany(_ => _.FixableDiagnosticIds).ToHashSet();

            foreach (var descriptor in AllAnalyzers.Select(_ => _.GetType().GetProperty("Rule", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_)).OfType<DiagnosticDescriptor>().OrderBy(_ => _.Id).ThenBy(_ => _.Category))
            {
                if (category != descriptor.Category)
                {
                    category = descriptor.Category;

                    markdownBuilder
                        .AppendLine()
                        .AppendLine("### " + category)
                        .AppendFormat(tableFormat, "ID", "Title", "Enabled by default", "CodeFix available")
                        .AppendFormat(tableFormat, ":-", ":----", ":----------------:", ":---------------:");
                }

                const string Check = "&#x2713;";
                const string NoCheck = "\\-";

                markdownBuilder.AppendFormat(
                                             tableFormat,
                                             descriptor.Id,
                                             descriptor.Title.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"),
                                             descriptor.IsEnabledByDefault ? Check : NoCheck,
                                             codeFixProviders.Contains(descriptor.Id) ? Check : NoCheck);
            }

            var markdown = markdownBuilder.ToString();

            File.WriteAllText(@"z:\test.md", markdown);

            Assert.That(markdown, Is.Empty);
        }

        private static int GetDiagnosticIdStartingNumber(Analyzer analyzer) => analyzer.GetType().Namespace switch
                                                                                                                    {
                                                                                                                        "MiKoSolutions.Analyzers.Rules.Documentation" => 2,
                                                                                                                        "MiKoSolutions.Analyzers.Rules.Maintainability" => 3,
                                                                                                                        "MiKoSolutions.Analyzers.Rules.Metrics" => 0,
                                                                                                                        "MiKoSolutions.Analyzers.Rules.Naming" => 1,
                                                                                                                        "MiKoSolutions.Analyzers.Rules.Ordering" => 4,
                                                                                                                        "MiKoSolutions.Analyzers.Rules.Performance" => 5,
                                                                                                                        _ => -1,
                                                                                                                    };

        private static Analyzer[] CreateAllAnalyzers()
        {
            // !! Awful HACKs!!
            // these line are required to allow the tests to run successfully
            NamingLengthAnalyzer.EnabledPerDefault = true;
            MiKo_2306_CommentEndsWithPeriodAnalyzer.EnabledPerDefault = true;
            MiKo_3030_MethodsFollowLawOfDemeterAnalyzer.EnabledPerDefault = true;

            //// TODO: RKN Fix Markdown for those that are enabled

            var baseType = typeof(Analyzer);

            var allAnalyzers = baseType.Assembly.GetExportedTypes()
                                       .Where(_ => _.IsAbstract is false && baseType.IsAssignableFrom(_))
                                       .Select(_ => (Analyzer)_.GetConstructor(Type.EmptyTypes).Invoke(null))
                                       .OrderBy(_ => _.DiagnosticId)
                                       .ToArray();

            return allAnalyzers;
        }

        private static CodeFixProvider[] CreateAllCodeFixProviders()
        {
            var baseType = typeof(CodeFixProvider);

            var allAnalyzers = typeof(Analyzer).Assembly.GetExportedTypes()
                                               .Where(_ => _.IsAbstract is false && baseType.IsAssignableFrom(_))
                                               .Select(_ => (CodeFixProvider)_.GetConstructor(Type.EmptyTypes).Invoke(null))
                                               .OrderBy(_ => _.GetType().Name)
                                               .ToArray();

            return allAnalyzers;
        }
    }
}