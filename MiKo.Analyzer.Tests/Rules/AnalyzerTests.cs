using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules
{
    [TestFixture]
    public static class AnalyzerTests
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager(typeof(Resources));

        [TestCaseSource(nameof(CreateAllAnalyzers))]
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

        [TestCaseSource(nameof(CreateAllAnalyzers))]
        public static void Analyzers_start_with_their_Id(Analyzer analyzer) => Assert.That(analyzer.GetType().Name, Is.Not.Null.And.StartsWith(analyzer.DiagnosticId + "_"));

        [Test]
        public static void Analyzers_have_unique_Ids()
        {
            var findings = new List<string>();

            var ids = new Dictionary<string, string>();
            foreach (var analyzer in CreateAllAnalyzers())
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

        [TestCaseSource(nameof(CreateAllAnalyzers))]
        public static void Analyzer_starts_with_correct_number(Analyzer analyzer)
        {
            var id = GetDiagnosticIdStartingNumber(analyzer);

            Assert.That(analyzer.DiagnosticId, Is.Not.Null.And.StartsWith("MiKo_" + id));
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

        private static IEnumerable<Analyzer> CreateAllAnalyzers()
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