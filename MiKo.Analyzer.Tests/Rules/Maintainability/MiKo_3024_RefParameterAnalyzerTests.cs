using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3024_RefParameterAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_that_has_no_ref_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int DoSomething(CancellationToken token) => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_a_ref_parameter_for_a_struct() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(ref int i) => i = 42;
}
");

        [Test]
        public void An_issue_is_reported_for_class_method_that_has_a_ref_parameter_for_a_reference_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(ref object o) => o = new object();
}
");

        [Test]
        public void An_issue_is_reported_for_interfce_method_that_has_a_ref_parameter_for_a_reference_type() => An_issue_is_reported_for(@"
using System;

public interface TestMe
{
    void DoSomething(ref object o);
}
");

        protected override string GetDiagnosticId() => MiKo_3024_RefParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3024_RefParameterAnalyzer();
    }
}