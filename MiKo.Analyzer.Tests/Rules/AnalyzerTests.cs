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
        [Test]
        public static void Resources_contains_texts()
        {
            var resourceManager = new ResourceManager(typeof(Resources));
            var analyzerBaseType = typeof(Analyzer);

            var allAnalyzers = analyzerBaseType.Assembly.GetExportedTypes()
                                               .Where(_ => !_.IsAbstract && analyzerBaseType.IsAssignableFrom(_))
                                               .Select(_ => (Analyzer)_.GetConstructor(Type.EmptyTypes).Invoke(null))
                                               .ToList();

            var missingEntries = new List<string>();
            foreach (var analyzer in allAnalyzers)
            {
                foreach (var name in new[]
                                         {
                                             analyzer.DiagnosticId + "_Description",
                                             analyzer.DiagnosticId + "_MessageFormat",
                                             analyzer.DiagnosticId + "_Title",
                                         })
                {
                    if (string.IsNullOrWhiteSpace(resourceManager.GetString(name)))
                        missingEntries.Add(name);
                }
            }

            missingEntries.Sort();

            Assert.That(missingEntries, Is.Empty);
        }
    }
}