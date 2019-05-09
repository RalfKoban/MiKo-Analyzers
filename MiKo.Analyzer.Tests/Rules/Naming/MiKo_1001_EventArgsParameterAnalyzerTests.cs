using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1001_EventArgsParameterAnalyzerTests : CodeFixVerifier
    {
        [TestCase("object s, EventArgs args")]
        [TestCase("DependencyObject d, DependencyPropertyChangedEventArgs args")]
        public void No_issue_is_reported_for_event_handling_method(string parameters) => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething(" + parameters + @") { }
    }
}
");

        [TestCase("")]
        [TestCase("int args")]
        [TestCase("object s")]
        [TestCase("object s, int args, object whatever")]
        [TestCase("object whatever, object s, int args")]
        public void No_issue_is_reported_for_non_matching_method(string parameters) => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        [TestCase("EventArgs args")]
        [TestCase("EventArgs args, object s")]
        [TestCase("EventArgs e, EventArgs a")]
        [TestCase("object s, EventArgs args, object whatever")]
        [TestCase("object whatever, object s, EventArgs args")]
        [TestCase("DependencyPropertyChangedEventArgs args")]
        [TestCase("DependencyPropertyChangedEventArgs args, object s")]
        [TestCase("DependencyPropertyChangedEventArgs e, DependencyPropertyChangedEventArgs a")]
        [TestCase("object s, DependencyPropertyChangedEventArgs args, object whatever")]
        [TestCase("object whatever, object s, DependencyPropertyChangedEventArgs args")]
        public void An_issue_is_reported_for_matching_method(string parameters) => An_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething(" + parameters + @") { }
    }
}
");

        [TestCase("EventArgs e")]
        [TestCase("EventArgs e, string a")]
        [TestCase("EventArgs e1, EventArgs e2")]
        [TestCase("DependencyPropertyChangedEventArgs e")]
        [TestCase("DependencyPropertyChangedEventArgs e, string a")]
        [TestCase("DependencyPropertyChangedEventArgs e1, DependencyPropertyChangedEventArgs e2")]
        public void No_issue_is_reported_for_correctly_named_method(string parameters) => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void DoSomething(" + parameters + @") { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_setter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public EventArgs Property { get; set; }
}
");
        protected override string GetDiagnosticId() => MiKo_1001_EventArgsParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1001_EventArgsParameterAnalyzer();
    }
}