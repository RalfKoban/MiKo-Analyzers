﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidTypes =
                                                      [
                                                          "IValueConverter",
                                                          "IMultiValueConverter",
                                                          "System.Windows.Data.IValueConverter",
                                                          "System.Windows.Data.IMultiValueConverter",
                                                      ];

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
        public void No_issue_is_reported_for_undocumented_value_converter_class_([ValueSource(nameof(ValidTypes))] string interfaceName) => No_issue_is_reported_for(@"
using System;
using System.Windows.Data;

public class TestMe : " + interfaceName + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_value_converter_class_documentation_that_starts_with_phrase_([ValueSource(nameof(ValidTypes))] string interfaceName) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_value_converter_class_documentation_that_starts_with_invalid_phrase_([ValueSource(nameof(ValidTypes))] string interfaceName) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Converts something.
/// </summary>
public class TestMe : " + interfaceName + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_value_converter_class_documentation_that_starts_with_see_cref() => An_issue_is_reported_for(@"
using System;
using System.Windows.Data;

/// <summary>
/// <see cref=""TestMe""/>
/// </summary>
public class TestMe : IValueConverter
{
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System;
using System.Windows.Data;

/// <summary>
/// Something.
/// </summary>
public class TestMe : IValueConverter
{
}
";

            const string FixedCode = @"
using System;
using System.Windows.Data;

/// <summary>
/// Represents a converter that converts something.
/// </summary>
public class TestMe : IValueConverter
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2048_CodeFixProvider();
    }
}