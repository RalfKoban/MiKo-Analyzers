using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2077_SummaryShouldNotUseCodeTagAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_items() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler<T> MyEvent;

    public void DoSomething() { }

    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_items_without_code() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// </summary>
public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public event EventHandler<T> MyEvent;

    /// <summary>
    /// Some documentation.
    /// </summary>
    public void DoSomething() { }

    /// <summary>
    /// Some documentation.
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_items_with_correct_example_docu() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// </summary>
/// <example>
/// The following example demonstrates its usage.
/// <code>
/// var x = 42;
/// </code>
/// </example>
public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// The following example demonstrates its usage.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </example>
    public event EventHandler<T> MyEvent;

    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// The following example demonstrates its usage.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </example>
    public void DoSomething() { }

    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// The following example demonstrates its usage.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </example>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documented_summary_on_type() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// <code>
/// var x = 42;
/// </code>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documented_summary_on_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documented_summary_on_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documented_summary_on_event() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </summary>
    public event EventHandler<T> MyEvent;
}
");

        protected override string GetDiagnosticId() => MiKo_2077_SummaryShouldNotUseCodeTagAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2077_SummaryShouldNotUseCodeTagAnalyzer();
    }
}