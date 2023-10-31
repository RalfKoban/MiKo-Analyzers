using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2047_AttributeSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidPhrases =
                                                        {
                                                            "Specifies ",
                                                            "Indicates ",
                                                            "Defines ",
                                                            "Provides ",
                                                            "Represents ",
                                                            "Allows ",
                                                            "Marks",
                                                        };

        private static readonly string[] InvalidPhrases = { "The ", "Attribute ", };

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
        public void No_issue_is_reported_for_attribute_class_documentation_that_starts_with_([ValueSource(nameof(ValidPhrases))] string phrase) => No_issue_is_reported_for(@"
using System;

/// <summary>
/// " + phrase + @" to do something.
/// </summary>
public class TestMe : Attribute
{
}
");

        [Test]
        public void An_issue_is_reported_for_attribute_class_documentation_that_starts_with_([ValueSource(nameof(InvalidPhrases))] string phrase) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// " + phrase + @" to do something.
/// </summary>
public class TestMe : Attribute
{
}
");

        protected override string GetDiagnosticId() => MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer();
    }
}