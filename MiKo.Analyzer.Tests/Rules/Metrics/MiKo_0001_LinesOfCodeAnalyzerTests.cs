using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;
using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public class MiKo_0001_LinesOfCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Valid_files_are_not_reported_as_warnings([ValueSource(nameof(ValidFiles))] string fileContent) => No_issue_is_reported_for(fileContent);

        [Test]
        public void Method_with_long_if_statement_is_reported() => An_issue_is_reported_for(@"
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
        public void Method_with_long_switch_statement_is_reported() => An_issue_is_reported_for(@"
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
        public void Method_with_long_try_statement_is_reported() => An_issue_is_reported_for(@"
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
        public void Method_with_long_catch_statement_is_reported() => An_issue_is_reported_for(@"
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
        public void Method_with_long_finally_statement_is_reported() => An_issue_is_reported_for(@"
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

        protected override Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0001_LinesOfCodeAnalyzer { MaxLinesOfCode = 3 };

        protected override string GetDiagnosticId() => MiKo_0001_LinesOfCodeAnalyzer.Id;

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
    }
}
