using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3025_ReuseParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_no_reused_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        var j = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_reused_ref_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(ref int i)
    {
        i = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_reused_out_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(out int i)
    {
        i = 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_has_reused_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        i = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_assignment() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int i = 42;
}
");

        [Test]
        public void No_issue_is_reported_for_property_assignment() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int Property { get; } = 42;
}
");

        [Test]
        public void No_issue_is_reported_for_complex_property_assignment() => No_issue_is_reported_for(@"
using System;

public class Property
{
    public int Value { get; set; }
}

public class TestMe
{
    public Property MyProperty { get; } = new Property
                                              {
                                                  Value = 42
                                              };
}
");

        [Test]
        public void No_issue_is_reported_for_property_and_field_assignment_in_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe()
    {
        Property = 42;
        field = 42;
    }

    public int Property { get; }
    private int field;
}
");

        protected override string GetDiagnosticId() => MiKo_3025_ReuseParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3025_ReuseParameterAnalyzer();
    }
}