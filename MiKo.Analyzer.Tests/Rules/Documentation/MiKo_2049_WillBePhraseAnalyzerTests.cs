using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2049_WillBePhraseAnalyzerTests : CodeFixVerifier
    {
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
        public void An_issue_is_reported_for_correctly_documented_summary_on_class([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

/// <" + tag + @">It will be.</" + tag + @">
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_correctly_documented_summary_on_method([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">It will be.</" + tag + @">
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_correctly_documented_summary_on_property([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">It will be.</" + tag + @">
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_correctly_documented_summary_on_event([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">It will be.</" + tag + @">
    public event EventHandler<T> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_correctly_documented_summary_on_field([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">It will be.</" + tag + @">
    private bool m_field;
}
");

        protected override string GetDiagnosticId() => MiKo_2049_WillBePhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2049_WillBePhraseAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> XmlTags() => new[] { "summary", "remarks", "returns", "example", "value", "exception" };
    }
}