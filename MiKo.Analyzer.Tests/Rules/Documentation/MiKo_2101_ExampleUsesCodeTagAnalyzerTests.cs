using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2101_ExampleUsesCodeTagAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_documented_items_without_example_docu() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_wrong_documented_example_docu_on_type() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// </summary>
/// <example>
/// var x = 42;
/// </example>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documented_example_docu_on_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// var x = 42;
    /// </example>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documented_example_docu_on_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// var x = 42;
    /// </example>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_documented_example_docu_on_event() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// var x = 42;
    /// </example>
    public event EventHandler<T> MyEvent;
}
");

        protected override string GetDiagnosticId() => MiKo_2101_ExampleUsesCodeTagAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2101_ExampleUsesCodeTagAnalyzer();
    }
}