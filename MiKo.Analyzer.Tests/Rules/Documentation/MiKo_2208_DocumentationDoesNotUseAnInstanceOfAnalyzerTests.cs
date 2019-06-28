using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Phrases =
            {
                "An instance of ",
                "A instance of ",
                "The instance of ",
                "An object of ",
                "A object of ",
                "The object of ",

                "handles an instance of something",
                "handles a instance of something",
                "handles the instance of something",
                "handles an object of something",
                "handles a object of something",
                "handles the object of something",
            };

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

        [Test, Combinatorial]
        public void An_issue_is_reported_for_correctly_documented_summary_on_class([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

/// <" + tag + @">" + phrase + @".</" + tag + @">
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_correctly_documented_summary_on_method([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">" + phrase + @".</" + tag + @">
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_correctly_documented_summary_on_property([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">" + phrase + @".</" + tag + @">
    public int Age { get; set; }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_correctly_documented_summary_on_event([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">" + phrase + @".</" + tag + @">
    public event EventHandler<T> MyEvent;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_correctly_documented_summary_on_field([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + @">" + phrase + @".</" + tag + @">
    private bool m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_summary_on_factory_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Creates an instance of the <see cref=""TestMe"" /> type.
    /// </summary>
    public TestMe DoSomething() { }
}
");

        protected override string GetDiagnosticId() => MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer();
    }
}