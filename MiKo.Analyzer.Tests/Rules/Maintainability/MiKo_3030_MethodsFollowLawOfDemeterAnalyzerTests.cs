using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3030_MethodsFollowLawOfDemeterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_1st_level_SimpleMemberAccessExpression_inside_a_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public TestMe DoSomething()
    {
        return Sub;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_2nd_level_SimpleMemberAccessExpression_inside_a_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public TestMe DoSomething()
    {
        return Sub.Sub;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_2nd_level_SimpleMemberAccessExpression_inside_a_method_using_conditionals() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public TestMe DoSomething()
    {
        return Sub?.Sub;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_2nd_level_SimpleMemberAccessExpression_inside_a_method_invocation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public string DoSomething()
    {
        return Sub.Sub.DoSomething();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_3rd_level_SimpleMemberAccessExpression_inside_a_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public TestMe DoSomething()
    {
        return Sub.Sub.Sub;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_3rd_level_SimpleMemberAccessExpression_inside_a_method_using_conditionals() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public TestMe DoSomething()
    {
        return Sub?.Sub?.Sub;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_3rd_level_SimpleMemberAccessExpression_inside_a_method_using_nested_classes() => No_issue_is_reported_for(@"
using System;

public static class A
{
    public static class B
    {
        public const string C = ""some text""
    }
}

public class TestMe
{
    public string DoSomething()
    {
        return A.B.C;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_4th_level_SimpleMemberAccessExpression_inside_a_method_using_conditionals() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public TestMe DoSomething()
    {
        return Sub?.Sub.Sub.Sub;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_4th_level_SimpleMemberAccessExpression_inside_a_method_using_nested_classes() => No_issue_is_reported_for(@"
using System;

public static class A
{
    public static class B
    {
        public static class C
        {
            public const string D = ""some text""
        }
    }
}

public class TestMe
{
    public string DoSomething()
    {
        return A.B.C.D;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3030_MethodsFollowLawOfDemeterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3030_MethodsFollowLawOfDemeterAnalyzer();
    }
}