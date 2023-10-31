using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2206_FlagAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = { "summary", "remarks", "returns", "example", "value", "exception" };

        [Test]
        public void No_issue_is_reported_for_undocumented_items() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler<T> MyEvent;

    public void DoSomething() { }

    public int Age { get; set; }

    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_items() => No_issue_is_reported_for(@"
using System;

/// <summary>Does something.</summary>
/// <remarks>Does something.</remarks>
public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    public event EventHandler<T> MyEvent;

    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    public void DoSomething() { }

    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    public int Age { get; set; }

    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    private bool m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_class_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

/// <" + tag + ">Its flag.</" + tag + @">
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Its flag.</" + tag + @">
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Its flag.</" + tag + @">
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_event_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Its flag.</" + tag + @">
    public event EventHandler<T> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_field_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Its flag.</" + tag + @">
    private bool m_field;
}
");

        protected override string GetDiagnosticId() => MiKo_2206_FlagAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2206_FlagAnalyzer();
    }
}