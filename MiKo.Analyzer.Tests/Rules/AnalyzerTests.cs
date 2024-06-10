using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules
{
    [TestFixture, Category("Always impacted")]
    public static class AnalyzerTests
    {
        private static readonly ResourceManager ResourceManager = new(typeof(Resources));
        private static readonly Analyzer[] AllAnalyzers = CreateAllAnalyzers();
        private static readonly CodeFixProvider[] AllCodeFixProviders = CreateAllCodeFixProviders();

        [Ignore("Just for now")]
        [TestCase("TODO"), Explicit, Timeout(1 * 60 * 60 * 1000)]
        public static void Performance(string path)
        {
            // ncrunch: no coverage start
            var files = GetDocuments(path).ToList();
            var sources = files.Select(File.ReadAllText).ToArray();

            var results = DiagnosticVerifier.GetDiagnostics(sources, AllAnalyzers.Cast<DiagnosticAnalyzer>().ToArray());

            Assert.That(results.Count, Is.Zero);

            static IEnumerable<string> GetDocuments(string path)
            {
                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    foreach (var document in GetDocuments(directory))
                    {
                        yield return document;
                    }
                }

                foreach (var file in Directory.EnumerateFiles(path, "*.cs"))
                {
                    yield return file;
                }
            }

            // ncrunch: no coverage end
        }

        [Test]
        public static void Resources_contains_texts() => Assert.Multiple(() =>
                                                                              {
                                                                                  foreach (var analyzer in AllAnalyzers)
                                                                                  {
                                                                                      var findings = new[]
                                                                                                         {
                                                                                                             analyzer.DiagnosticId + "_Description",
                                                                                                             analyzer.DiagnosticId + "_MessageFormat",
                                                                                                             analyzer.DiagnosticId + "_Title",
                                                                                                         }
                                                                                                     .Where(_ => string.IsNullOrWhiteSpace(ResourceManager.GetString(_, CultureInfo.CurrentUICulture)))
                                                                                                     .ToList();

                                                                                      findings.Sort();

                                                                                      Assert.That(findings, Is.Empty);
                                                                                  }
                                                                              });

        [Test, Explicit, Ignore("Shall be run manually")]
        public static void Resources_contains_no_multiple_consecutive_spaces() => Assert.Multiple(() =>
                                                                                                       {
                                                                                                           foreach (var analyzer in AllAnalyzers)
                                                                                                           {
                                                                                                               var findings = new[]
                                                                                                                                  {
                                                                                                                                      analyzer.DiagnosticId + "_Description",
                                                                                                                                      analyzer.DiagnosticId + "_MessageFormat",
                                                                                                                                      analyzer.DiagnosticId + "_Title",
                                                                                                                                      analyzer.DiagnosticId + "_CodeFixTitle",
                                                                                                                                  }
                                                                                                                              .Where(_ => ResourceManager.GetString(_, CultureInfo.CurrentUICulture)?.Contains("  ") is true)
                                                                                                                              .ToList();

                                                                                                               findings.Sort();

                                                                                                               Assert.That(findings, Is.Empty);
                                                                                                           }
                                                                                                       });

        [Test]
        public static void Titles_should_not_end_with_dot() => Assert.Multiple(() =>
                                                                                    {
                                                                                        foreach (var analyzer in AllAnalyzers)
                                                                                        {
                                                                                            var key = analyzer.DiagnosticId + "_Title";

                                                                                            var title = ResourceManager.GetString(key, CultureInfo.CurrentUICulture);

                                                                                            Assert.Multiple(() =>
                                                                                                                 {
                                                                                                                     Assert.That(title, Does.Not.StartWith(" "), $"'{key}' should not start with whitespace.");
                                                                                                                     Assert.That(title, Does.Not.EndWith(".").And.Not.EndWith(" "), $"'{key}' should not end with a dot or whitespace.");
                                                                                                                 });
                                                                                        }
                                                                                    });

        [Test]
        public static void Descriptions_should_end_with_dot() => Assert.Multiple(() =>
                                                                                      {
                                                                                          foreach (var analyzer in AllAnalyzers)
                                                                                          {
                                                                                              var key = analyzer.DiagnosticId + "_Description";

                                                                                              Assert.That(ResourceManager.GetString(key, CultureInfo.CurrentUICulture), Does.EndWith(".").Or.EndsWith(")"), $"'{key}' is incorrect.{Environment.NewLine}");
                                                                                          }
                                                                                      });

        [Test]
        public static void Messages_should_not_end_with_dot() => Assert.Multiple(() =>
                                                                                      {
                                                                                          foreach (var analyzer in AllAnalyzers)
                                                                                          {
                                                                                              var key = analyzer.DiagnosticId + "_MessageFormat";

                                                                                              var message = ResourceManager.GetString(key, CultureInfo.CurrentUICulture);

                                                                                              Assert.Multiple(() =>
                                                                                                                   {
                                                                                                                       Assert.That(message, Does.Not.StartWith(" "), $"'{key}' should not start with whitespace.");
                                                                                                                       Assert.That(message, Does.Not.EndWith(".").And.Not.EndWith(" "), $"'{key}' should not end with a dot or whitespace.");
                                                                                                                   });
                                                                                          }
                                                                                      });

        [Test]
        public static void CodeFixTitles_should_not_end_with_dot() => Assert.Multiple(() =>
                                                                                           {
                                                                                               foreach (var analyzer in AllAnalyzers)
                                                                                               {
                                                                                                   var key = analyzer.DiagnosticId + "_CodeFixTitle";

                                                                                                   var codefixTitle = ResourceManager.GetString(key, CultureInfo.CurrentUICulture);

                                                                                                   Assert.Multiple(() =>
                                                                                                                        {
                                                                                                                            Assert.That(codefixTitle, Does.Not.StartWith(" "), $"'{key}' should not start with whitespace.");
                                                                                                                            Assert.That(codefixTitle, Does.Not.EndWith(".").And.Not.EndWith(" "), $"'{key}' should not end with a dot or whitespace.");
                                                                                                                        });
                                                                                               }
                                                                                           });

        [Test, Ignore("Just to check from time to time whether the texts are acceptable or need to be rephrased.")]
        public static void Messages_should_not_contain_([Values("shall", "should")] string word)
        {
            Assert.Multiple(() =>
                                 {
                                     foreach (var analyzer in AllAnalyzers)
                                     {
                                         Assert.That(ResourceManager.GetString(analyzer.DiagnosticId + "_MessageFormat", CultureInfo.CurrentUICulture), Does.Not.Contain(word));
                                     }
                                 });
        }

        [Test]
        public static void Analyzers_start_with_their_Id() => Assert.Multiple(() =>
                                                                                   {
                                                                                       foreach (var analyzer in AllAnalyzers)
                                                                                       {
                                                                                           Assert.That(analyzer.GetType().Name, Is.Not.Null.And.StartsWith(analyzer.DiagnosticId + "_"));
                                                                                       }
                                                                                   });

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
        public static void Analyzer_starts_with_correct_number() => Assert.Multiple(() =>
                                                                                         {
                                                                                             foreach (var analyzer in AllAnalyzers)
                                                                                             {
                                                                                                 var id = GetDiagnosticIdStartingNumber(analyzer);

                                                                                                 Assert.That(analyzer.DiagnosticId, Is.Not.Null.And.StartsWith("MiKo_" + id));
                                                                                             }
                                                                                         });

        [Test]
        public static void Analyzer_are_marked_with_DiagnosticAnalyzer_attribute()
        {
            Assert.Multiple(() =>
                                 {
                                     foreach (var analyzer in AllAnalyzers)
                                     {
                                         Assert.That(analyzer.GetType(), Has.Attribute<DiagnosticAnalyzerAttribute>().With.Property(nameof(DiagnosticAnalyzerAttribute.Languages)).EquivalentTo(new[] { LanguageNames.CSharp }));
                                     }
                                 });
        }

        [Test]
        public static void CodeFixProvider_are_marked_with_ExportCodeFixProvider_attribute()
        {
            Assert.Multiple(() =>
                                 {
                                     foreach (var provider in AllCodeFixProviders)
                                     {
                                         var type = provider.GetType();

                                         Assert.That(type, Has.Attribute<ExportCodeFixProviderAttribute>().With.Property(nameof(ExportCodeFixProviderAttribute.Name)).EqualTo(type.Name));
                                     }
                                 });
        }

        [Test]
        public static void CodeFixProviders_use_Id_of_corresponding_Analyzer()
        {
            var map = AllAnalyzers.ToDictionary(_ => _.DiagnosticId);

            Assert.Multiple(() =>
                                 {
                                     foreach (var provider in AllCodeFixProviders)
                                     {
                                         var id = provider.FixableDiagnosticIds.First();

                                         Assert.That(map.ContainsKey(id), Is.True, $"Analyzer for '{id}' missing");
                                     }
                                 });
        }

        [Test]
        public static void CodeFixProvider_uses_simplified_name() => Assert.Multiple(() =>
                                                                                          {
                                                                                              foreach (var provider in AllCodeFixProviders)
                                                                                              {
                                                                                                  var id = provider.FixableDiagnosticIds.First();

                                                                                                  Assert.That(provider.GetType().Name, Does.StartWith(id).And.EndsWith("_CodeFixProvider"));
                                                                                              }
                                                                                          });

        [Test]
        public static void CodeFixProvider_has_a_title()
        {
            Assert.Multiple(() =>
                                 {
                                     foreach (var provider in AllCodeFixProviders)
                                     {
                                         var resourceKey = CreateResourceKey(provider);

                                         var expectedTitle = Resources.ResourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture) ?? "< - missing title - >";

                                         var codeFixTitle = provider.GetType().GetProperty("Title", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(provider).ToString();

                                         var parts = expectedTitle.FormatWith('|').Split('|');

                                         if (parts.Length <= 1)
                                         {
                                             Assert.That(codeFixTitle, Is.EqualTo(expectedTitle), $"No codefix title found at all for {resourceKey}");
                                         }
                                         else
                                         {
                                             Assert.That(codeFixTitle, Is.Not.EqualTo(expectedTitle), $"Missing phrase for {resourceKey}");
                                             Assert.That(codeFixTitle, Does.StartWith(parts[0]).And.EndWith(parts[1]), $"Codefix title missing for {resourceKey}");
                                         }
                                     }
                                 });

            static string CreateResourceKey(CodeFixProvider codeFixProvider)
            {
                var resourceKey = codeFixProvider.GetType().Name.Replace("_CodeFixProvider", string.Empty);

                foreach (var x in codeFixProvider.FixableDiagnosticIds)
                {
                    var number = x.Replace("MiKo", string.Empty);
                    resourceKey = resourceKey.Replace(number, string.Empty);
                }

                var id = codeFixProvider.FixableDiagnosticIds.First();

                return id + "_CodeFixTitle" + resourceKey.Replace("MiKo", string.Empty);
            }
        }

        [Test, Ignore("Just to find gaps")]
        public static void Gaps_in_Analyzer_numbers_([Range(1, 6, 1)] int i)
        {
            var gaps = new List<string>();

            var diagnosticIds = AllAnalyzers.Select(_ => _.DiagnosticId).ToArray();

            var thousand = i * 1000;

            for (var j = 0; j < thousand; j++)
            {
                var number = thousand + j;
                var prefix = $"MiKo_{number:####}";

                if (diagnosticIds.TrueForAll(_ => _.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) is false))
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
                                                     .AppendLine(CultureInfo.CurrentCulture, $"The following tables list all the {AllAnalyzers.Length} rules that are currently provided by the analyzer.");

            var category = string.Empty;
            var tableFormat = "|{0}|{1}|{2}|{3}|" + Environment.NewLine;

            var codeFixProviders = EnumerableExtensions.ToHashSet(AllCodeFixProviders.SelectMany(_ => _.FixableDiagnosticIds));

            foreach (var descriptor in AllAnalyzers.Select(_ => _.GetType().GetProperty("Rule", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_)).OfType<DiagnosticDescriptor>().OrderBy(_ => _.Id).ThenBy(_ => _.Category))
            {
                if (category != descriptor.Category)
                {
                    category = descriptor.Category;

                    markdownBuilder.AppendLine()
                                   .AppendLine("### " + category)
                                   .AppendFormat(CultureInfo.CurrentCulture, tableFormat, "ID", "Title", "Enabled by default", "CodeFix available")
                                   .AppendFormat(CultureInfo.CurrentCulture, tableFormat, ":-", ":----", ":----------------:", ":---------------:");
                }

                const string Check = "&#x2713;";
                const string NoCheck = "\\-";

                markdownBuilder.AppendFormat(
                                         CultureInfo.CurrentCulture,
                                         tableFormat,
                                         descriptor.Id,
                                         descriptor.Title.ToString(CultureInfo.CurrentCulture).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("|", "&#124;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("#", "&num;"),
                                         descriptor.IsEnabledByDefault ? Check : NoCheck,
                                         codeFixProviders.Contains(descriptor.Id) ? Check : NoCheck);
            }

            var markdown = markdownBuilder.ToString();

            File.WriteAllText(@"z:\test.md", markdown);

            Assert.That(markdown, Is.Empty);
        }

        private static int GetDiagnosticIdStartingNumber(Analyzer analyzer) => analyzer.GetType().Namespace switch
                                                                                                                   {
                                                                                                                       "MiKoSolutions.Analyzers.Rules.Metrics" => 0,
                                                                                                                       "MiKoSolutions.Analyzers.Rules.Naming" => 1,
                                                                                                                       "MiKoSolutions.Analyzers.Rules.Documentation" => 2,
                                                                                                                       "MiKoSolutions.Analyzers.Rules.Maintainability" => 3,
                                                                                                                       "MiKoSolutions.Analyzers.Rules.Ordering" => 4,
                                                                                                                       "MiKoSolutions.Analyzers.Rules.Performance" => 5,
                                                                                                                       "MiKoSolutions.Analyzers.Rules.Spacing" => 6,
                                                                                                                       _ => -1,
                                                                                                                   };

        private static Analyzer[] CreateAllAnalyzers()
        {
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