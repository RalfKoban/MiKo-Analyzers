using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1049_RequirementTermAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_symbols() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_something;

    public event EventHandler SomethingEvent;

    public bool Something { get; set;}

    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_type([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class " + marker + @"TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_field([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_something" + marker + @";
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_const_field([ValueSource(nameof(Marker))] string marker) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const string Bla" + marker + @" = ""something"";
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_event([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Something" + marker + @"Event;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_property([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool " + marker + @"Something { get; set;}
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method([ValueSource(nameof(Marker))] string marker) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void " + marker + @"DoSomething() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1049_RequirementTermAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1049_RequirementTermAnalyzer();

        private static IEnumerable<string> Marker() => new[] { "Must", "Need", "Shall", "Should", "Will", "Would" };
    }
}