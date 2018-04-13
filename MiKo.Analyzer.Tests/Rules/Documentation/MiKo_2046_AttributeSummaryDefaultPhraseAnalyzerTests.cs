using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2046_AttributeSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_attribute_class() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some text.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_attribute_class() => No_issue_is_reported_for(@"
using System;

public class TestMe : Attribute
{
}
");

        [Test]
        public void No_issue_is_reported_for_attribute_class_documentation_that_starts_with([ValueSource(nameof(ValidPhrases))] string phrase) => No_issue_is_reported_for(@"
using System;

/// <summary>
/// " + phrase + @" to do something.
/// </summary>
public class TestMe : Attribute
{
}
");

        [Test]
        public void An_issue_is_reported_for_attribute_class_documentation_that_starts_with([ValueSource(nameof(InvalidPhrases))] string phrase) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// " + phrase + @" to do something.
/// </summary>
public class TestMe : Attribute
{
}
");

        protected override string GetDiagnosticId() => MiKo_2046_AttributeSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2046_AttributeSummaryDefaultPhraseAnalyzer();

        private static IEnumerable<string> ValidPhrases() => new []
                                                                 {
                                                                     "Specifies ",
                                                                     "Indicates ",
                                                                     "Defines ",
                                                                     "Provides ",
                                                                     "Represents ",
                                                                     "Allows ",
                                                                 };

        private static IEnumerable<string> InvalidPhrases() => new []
                                                                 {
                                                                     "The ",
                                                                     "Attribute ",
                                                                 };
    }
}