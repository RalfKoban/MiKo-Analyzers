using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2019_PassiveVerbSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Age { get; set; }

    public void DoSomething() { }

    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_class() => No_issue_is_reported_for(@"
using System;

using Bla
{
    /// <summary>
    /// Contains some test data.
    /// </summary>
    public class TestMe
    {
        /// <summary>
        /// Gets or sets some test data.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Performs some test data.
        /// </summary>
        public virtual void DoSomething() { }

        /// <summary>
        /// Allows to do some test data.
        /// </summary>
        public virtual void DoSomething2() { }

        /// <summary>
        /// Breaks to do some test data.
        /// </summary>
        public virtual void DoSomething2() { }

        /// <summary>
        /// Asynchronously provides some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Asynchronously stops some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Occurs after some test data.
        /// </summary>
        public event EventHandler MyEvent;
    }

    /// <summary>
    /// Contains some test data.
    /// </summary>
    public class TestMe2
    {
        /// <summary>
        /// Gets or sets some test data.
        /// </summary>
        public int Age { get; set; }

        /// <inheritdoc />
        public override void DoSomething() { }

        /// <summary>
        /// Asynchronously provides some test data.
        /// </summary>
        public Task DoSomethingAsync() { }

        /// <summary>
        /// Occurs after some test data.
        /// </summary>
        public event EventHandler MyEvent;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_class() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Provide some test data.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Perform some test data.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_async_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Asynchronously perform some test data.
    /// </summary>
    public Task DoSomethingAsync() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Get or sets some test data.
    /// </summary>
    public int Age { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_2019_PassiveVerbSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2019_PassiveVerbSummaryAnalyzer();
    }
}