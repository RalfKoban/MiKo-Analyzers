using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2054_ArgumentOutOfRangeExceptionPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_throwing_an_ArgumentOutOfRangeException() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// <paramref name=""o""/> is something.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_throwing_an_ArgumentOutOfRangeException_for_multiple_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// <paramref name=""o1""/> is something.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is something.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_throwing_an_ArgumentOutOfRangeException() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// The <paramref name=""o""/> is not set.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_throwing_an_ArgumentOutOfRangeException_without_paramref_tags() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// Thrown if something is not set.
    /// </exception>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_1st_parameter_throwing_an_ArgumentOutOfRangeException_for_multiple_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// The <paramref name=""o1""/> is not set.
    /// <para>-or-</para>
    /// <paramref name=""o2""/> is something.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_2nd_parameter_throwing_an_ArgumentOutOfRangeException_for_multiple_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// <paramref name=""o1""/> is something.
    /// <para>-or-</para>
    /// The <paramref name=""o2""/> is not set.
    /// </exception>
    public void DoSomething(object o1, object o2) { }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_property_without_setter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public object DoSomething { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_property_with_private_setter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <value>
    /// Something to return.
    /// </value>
    public object DoSomething { get; private set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_property_throwing_an_ArgumentOutOfRangeException() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// <paramref name=""value""/> is something.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property_throwing_an_ArgumentOutOfRangeException() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// The <paramref name=""value""/> is not set.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property_throwing_an_ArgumentOutOfRangeException_without_paramref_tags() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <exception cref=""ArgumentOutOfRangeException"">
    /// Thrown if something is not set.
    /// </exception>
    public object DoSomething { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_2054_ArgumentOutOfRangeExceptionPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2054_ArgumentOutOfRangeExceptionPhraseAnalyzer();
    }
}