using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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
        public void No_issue_is_reported_for_a_1st_level_SimpleMemberAccessExpression_inside_a_method_invocation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public string DoSomething()
    {
        return Sub.DoSomething();
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
        public void An_issue_is_reported_for_a_3rd_level_SimpleMemberAccessExpression_inside_a_method_invocation() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Sub { get; }

    public string DoSomething()
    {
        return Sub.Sub.Sub.DoSomething();
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

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_a_4th_level_SimpleMemberAccessExpression_inside_a_method_using_conditionals() => An_issue_is_reported_for(2, @"
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

        [Test]
        public void No_issue_is_reported_for_a_1st_level_ElementAccessExpression_inside_a_method() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> Subs { get; }

    public TestMe DoSomething()
    {
        return Subs[0];
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_2nd_level_ElementAccessExpression_inside_a_method() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> Subs { get; }

    public TestMe Sub { get; }

    public TestMe DoSomething()
    {
        return Subs[0].Sub;
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_a_3rd_level_ElementAccessExpression_inside_a_method() => An_issue_is_reported_for(3, @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<TestMe> Subs { get; }

    public TestMe Sub { get; }

    public TestMe DoSomething()
    {
        return Subs[0].Subs[0].Sub;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_NUnits_Constraint_approach() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Assert.That(42, Is.Not.Null.And.Not.Empty);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_full_qualified_enum() => No_issue_is_reported_for(@"
using System;

namespace Bla.Blubb.DiBlubb
{
    public enum MyTestEnum
    {
        None = 0,
    }

    public class TestMe
    {
        public MyTestEnum Value { get; set; }

        public void DoSomething()
        {
            Value = Bla.Blubb.DiBlubb.MyTestEnum.None;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3030_MethodsFollowLawOfDemeterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            MiKo_3030_MethodsFollowLawOfDemeterAnalyzer.EnabledPerDefault = true;

            Analyzer.Reset();

            return new MiKo_3030_MethodsFollowLawOfDemeterAnalyzer();
        }
    }
}