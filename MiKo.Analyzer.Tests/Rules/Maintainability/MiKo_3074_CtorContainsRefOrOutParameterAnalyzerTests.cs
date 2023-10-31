using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3074_CtorContainsRefOrOutParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_ref_or_out_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(ref int i, out object o)
    {
        o = null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe() { }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_with_non_ref_non_out_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe(int value) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_with_ref_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe(int value, ref int other) { }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_with_out_parameters() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe(int value, out object o) { }
}
");

        protected override string GetDiagnosticId() => MiKo_3074_CtorContainsRefOrOutParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3074_CtorContainsRefOrOutParameterAnalyzer();
    }
}