using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3031_ICloneableCloneAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_Clone_method_on_class_that_returns_void() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_Clone_method_on_class_that_returns_object() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object DoSomething()
    {
        return null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_custom_Clone_method_on_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe Clone()
    {
        return new TestMe();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_custom_static_Clone_method_on_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static TestMe Clone()
    {
        return new TestMe();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_custom_static_generic_Clone_method_on_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static T Clone<T>()
    {
        return new T();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_strange_custom_static_generic_Clone_method_on_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static object Clone<T>()
    {
        return new T();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_object_Clone_method_on_class() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Clone()
    {
        return new TestMe();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ICloneable_Clone_method_on_class() => An_issue_is_reported_for(@"
using System;

public class TestMe : ICloneable
{
    public object Clone()
    {
        return new TestMe();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_explicit_interface_ICloneable_Clone_method_on_class() => An_issue_is_reported_for(@"
using System;

public class TestMe : ICloneable
{
    object ICloneable.Clone()
    {
        return new TestMe();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_explicit_full_qualified_interface_ICloneable_Clone_method_on_class() => An_issue_is_reported_for(@"

public class TestMe : System.ICloneable
{
    object System.ICloneable.Clone()
    {
        return new TestMe();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3031_ICloneableCloneAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3031_ICloneableCloneAnalyzer();
    }
}