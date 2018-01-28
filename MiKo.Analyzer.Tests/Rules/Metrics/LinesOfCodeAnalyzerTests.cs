using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public class LinesOfCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Valid_files_are_not_reported_as_warnings([ValueSource(nameof(ValidFiles))] string fileContent) => VerifyCSharpDiagnostic(fileContent);

        private static IEnumerable<string> ValidFiles()
        {
            var contents = new List<string> { string.Empty };
            contents.AddRange(GetEmbeddedFileContents(".Valid.LoC_"));

            return contents;
        }

        [Test, Ignore("TODO")]
        public void Files_with_issues_are_reported_as_warnings([ValueSource(nameof(FilesWithIssuses))] string fileContent)
        {
            var expected = new DiagnosticResult
            {
                Id = "MiKo_Code_Analyzer",
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 15)
                    }
            };

            VerifyCSharpDiagnostic(fileContent, expected);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null; // return new MiKo_Code_AnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new LinesOfCodeAnalyzer { MaxLinesOfCode = 3 };

        private static IEnumerable<string> FilesWithIssuses()
        {
            return new List<string>(GetEmbeddedFileContents(".Errors.LoC_"));
        }

        private static IEnumerable<string> GetEmbeddedFileContents(string namePrefix)
        {
            var assembly = typeof(DiagnosticResult).Assembly;
            foreach (var resourceName in assembly.GetManifestResourceNames().Where(_ => _.Contains(namePrefix) && _.EndsWith("_cs")))
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                using (var reader = new StreamReader(stream))
                {
                    yield return reader.ReadToEnd();
                }
            }
        }
    }
}
