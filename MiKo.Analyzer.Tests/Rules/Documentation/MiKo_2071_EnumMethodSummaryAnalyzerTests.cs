using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2071_EnumMethodSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_items() => No_issue_is_reported_for(@"
public class TestMe
{
    public int SomethingProperty { get; set; }

    public int DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_items() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public StringComparison SomethingProperty { get; set; }

    /// <summary>
    /// Does something.
    /// </summary>
    public StringComparison DoSomething()
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether something is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether something is contained.
    /// </summary>
    public bool Contains() => true;
}
");
        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method([ValueSource(nameof(BooleanPhrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Gets " + phrase + @" something.
    /// </summary>
    public StringComparison DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property([ValueSource(nameof(BooleanPhrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Gets " + phrase + @" something.
    /// </summary>
    public StringComparison  SomethingProperty { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_2071_EnumMethodSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2071_EnumMethodSummaryAnalyzer();

        private static IEnumerable<string> BooleanPhrases() => new[]
                                                            {
                                                                "indicate if",
                                                                "indicate whether",
                                                                "indicates if",
                                                                "indicates whether",
                                                                "indicating if",
                                                                "indicating whether",
                                                            };
    }
}