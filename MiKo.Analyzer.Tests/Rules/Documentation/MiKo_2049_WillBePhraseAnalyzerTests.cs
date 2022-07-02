using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2049_WillBePhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = { "summary", "remarks", "returns", "example", "value", "exception" };

        private static readonly string[] Phrases = { "It will be.", "It will also be.", "It will as well be.", "It will return.", "It is something (will leave something)" };

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

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_class_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

/// <" + tag + @">" + phrase + "</" + tag + @">
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_method_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">" + phrase + "</" + tag + @">
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_property_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">" + phrase + "</" + tag + @">
    public int Age { get; set; }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_event_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">" + phrase + "</" + tag + @">
    public event EventHandler<T> MyEvent;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_field_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">" + phrase + "</" + tag + @">
    private bool m_field;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_interface_property_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public interface ITestMe
{
    /// <" + tag + @">" + phrase + "</" + tag + @">
    int Age { get; set; }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_interface_indexer_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public interface ITestMe
{
    /// <" + tag + @">" + phrase + "</" + tag + @">
    int this[int key] { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_2049_WillBePhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2049_WillBePhraseAnalyzer();
    }
}