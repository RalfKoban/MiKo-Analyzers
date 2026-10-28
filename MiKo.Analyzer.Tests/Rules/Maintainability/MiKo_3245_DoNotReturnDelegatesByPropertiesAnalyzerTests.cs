using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3245_DoNotReturnDelegatesByPropertiesAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Delegates = ["Action", "Action<int>", "Func<bool>", "Func<int, bool>"];

        [Test]
        public void No_issue_is_reported_for_property_returning_no_delegate() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int SomeProperty { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + delegateName + @" SomeProperty { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_init_property_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + delegateName + @" SomeProperty { get; init; }
}
");

        [Test]
        public void An_issue_is_reported_for_setter_only_property_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + delegateName + @" SomeProperty {  set; }
}
");

        [Test]
        public void An_issue_is_reported_for_getter_only_property_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + delegateName + @" SomeProperty { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_implicit_interface_implementation_of_property_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => No_issue_is_reported_for(@"
using System;

#pragma warning disable MiKo_3245

public interface IFoo
{
    " + delegateName + @" SomeProperty { get; }
}

#pragma warning restore MiKo_3245

public class TestMe : IFoo
{
    public " + delegateName + @" SomeProperty { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_explicit_interface_implementation_of_property_returning_delegate_([ValueSource(nameof(Delegates))] string delegateName) => No_issue_is_reported_for(@"
using System;

#pragma warning disable MiKo_3245

public interface IFoo
{
    " + delegateName + @" SomeProperty { get; }
}

#pragma warning restore MiKo_3245

public class TestMe : IFoo
{
    " + delegateName + @" IFoo.SomeProperty { get; }
}
");

        protected override string GetDiagnosticId() => MiKo_3245_DoNotReturnDelegatesByPropertiesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3245_DoNotReturnDelegatesByPropertiesAnalyzer();
    }
}