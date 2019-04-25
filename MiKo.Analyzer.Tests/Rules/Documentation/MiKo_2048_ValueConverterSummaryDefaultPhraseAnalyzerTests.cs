using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidTypes =
            {
                "IValueConverter",
                "IMultiValueConverter",
                "System.Windows.Data.IValueConverter",
                "System.Windows.Data.IMultiValueConverter",
            };

        [Test]
        public void No_issue_is_reported_for_non_value_converter_class() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some text.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_value_converter_class([ValueSource(nameof(ValidTypes))] string interfaceName) => No_issue_is_reported_for(@"
using System;
using System.Windows.Data;

public class TestMe : " + interfaceName + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_value_converter_class_documentation_that_starts_with_phrase([ValueSource(nameof(ValidTypes))] string interfaceName) => No_issue_is_reported_for(@"
using System;
using System.Windows.Data;

/// <summary>
/// Represents a converter that converts something.
/// </summary>
public class TestMe : " + interfaceName + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_value_converter_class_documentation_that_starts_with_invalid_phrase([ValueSource(nameof(ValidTypes))] string interfaceName) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Converts something.
/// </summary>
public class TestMe : " + interfaceName + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer();
    }
}