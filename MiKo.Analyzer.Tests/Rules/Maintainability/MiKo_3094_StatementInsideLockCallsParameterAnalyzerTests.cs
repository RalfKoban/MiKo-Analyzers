using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3094_StatementInsideLockCallsParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething()
    {
        lock (this)
        {
            DoSomething();
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_that_does_not_have_available_reference_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int x, int y)
    {
        lock (this)
        {
            DoSomething(1, 2);
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_that_does_not_use_available_parameters_inside_lock() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object a, object b, object c)
    {
        lock (this)
        {
            DoSomething(new object(), new object(), new object());
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_that_only_forwards_available_parameters_inside_lock() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object a, object b, object c)
    {
        lock (this)
        {
            DoSomething(new object(), b, new object());
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_that_calls_method_on_available_parameters_inside_lock() => An_issue_is_reported_for(@"
using System;

public class Data
{
    public object SomeData { get; set; }

    public Data SetSomeStuff() => null;
}

public class TestMe
{
    public bool DoSomething(int a, int b, Data data)
    {
        lock (this)
        {
            DoSomething(1, 2, data.GetSomeStuff());
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_that_calls_property_on_available_parameters_inside_lock() => An_issue_is_reported_for(@"
using System;

public class Data
{
    public Data SomeData { get; set; }

    public Data SetSomeStuff() => null;
}

public class TestMe
{
    public bool DoSomething(int a, int b, Data data)
    {
        lock (this)
        {
            DoSomething(1, 2, data.SomeData);
        }
    }
}");

        protected override string GetDiagnosticId() => MiKo_3094_StatementInsideLockCallsParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3094_StatementInsideLockCallsParameterAnalyzer();
    }
}