using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public void Valid_files_are_not_reported_as_warnings([ValueSource(nameof(ValidFiles))] string fileContent)
        {
            var results = GetDiagnostics(fileContent);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Method_with_long_if_statement_is_reported() => Issue_gets_reported(@"
    public class TypeWithMethod
    {
        public void Method()
        {
            if (true)
            {
                var x = 0;
                if (x == 0)
                {
                    return;
                }
            }
        }
    }
");

        [Test]
        public void Method_with_long_switch_statement_is_reported() => Issue_gets_reported(@"
    public class TypeWithMethod
    {
        public void Method()
        {
            switch (x)
            {
                case 1:
                    var x = 0;
                    break;
            }
        }
    }
");

        [Test]
        public void Method_with_long_try_statement_is_reported() => Issue_gets_reported(@"
    public class TypeWithMethod
    {
        public void Method()
        {
            try
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void Method_with_long_catch_statement_is_reported() => Issue_gets_reported(@"
    public class TypeWithMethod
    {
        public void Method()
        {
            catch (Exception ex)
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        [Test]
        public void Method_with_long_finally_statement_is_reported() => Issue_gets_reported(@"
    public class TypeWithMethod
    {
        public void Method()
        {
            finally
            {
                var x = 0;
                var y = 1;
                var z = x + y;
            }
        }
    }
");

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null; // return new MiKo_Code_AnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new LinesOfCodeAnalyzer { MaxLinesOfCode = 3 };

        private static IEnumerable<string> ValidFiles()
        {
            var contents = new List<string> { string.Empty };
            contents.AddRange(GetEmbeddedFileContents(".Valid.LoC_"));

            return contents;
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

        private void Issue_gets_reported(string fileContent)
        {
            var results = GetDiagnostics(fileContent);

            Assert.That(results.Single().Id, Is.EqualTo(LinesOfCodeAnalyzer.Id));
        }
    }
}
