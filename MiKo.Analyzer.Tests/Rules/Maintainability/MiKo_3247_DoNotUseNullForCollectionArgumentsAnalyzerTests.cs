using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_invocation_with_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(42);

    private void DoSomethingCore(int i) => { }
}
");

        [Test]
        public void No_issue_is_reported_for_invocation_with_null_as_non_collection() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(null);

    private void DoSomethingCore(object o) => { }
}
");

        [TestCase(""" "x", null """)]
        [TestCase(""" null, "x" """)]
        [TestCase(""" "x", null, "y" """)]
        [TestCase(""" "x", null, "y", null """)]
        public void No_issue_is_reported_for_null_in_expanded_params_arguments_(string arguments) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(" + arguments + @");

    private void DoSomethingCore(params object[] items) { }
}
");

        [Test]
        public void No_issue_is_reported_for_reordered_named_argument_with_null_as_non_collection() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(o: null, items: new int[0]);

    private void DoSomethingCore(int[] items, object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_reordered_named_argument_with_null_as_non_collection_when_other_parameter_is_collection() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(o: null, items: new int[0]);

    private void DoSomethingCore(object o, int[] items) { }
}
");

        [TestCase("int[] array")]
        [TestCase("params int[] array")]
        [TestCase("IEnumerable i")]
        [TestCase("IEnumerable<int> i")]
        [TestCase("List<int> i")]
        [TestCase("Dictionary<int, string> i")]
        public void An_issue_is_reported_for_invocation_with_null_as_(string parameter) => An_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(null);

    private void DoSomethingCore(" + parameter + @") => { }
}
");

        [Test]
        public void An_issue_is_reported_for_reordered_named_argument_with_null_as_collection() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(items: null, count: 1);

    private void DoSomethingCore(int count, int[] items) { }
}
");

        [Test]
        public void An_issue_is_reported_for_named_argument_with_null_as_collection_after_omitted_optional_argument() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => DoSomethingCore(items: null);

    private void DoSomethingCore(int count = 0, int[] items = null) { }
}
");

        protected override string GetDiagnosticId() => MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3247_DoNotUseNullForCollectionArgumentsAnalyzer();
    }
}