
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_1_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public int A() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_2_methods() => No_issue_is_reported_for(@"
public class TestMe
{
    public int A() => 42;

    public int B() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_1_public_and_1_private_method_sharing_same_name_and_1_public_method_in_between() => No_issue_is_reported_for(@"
public class TestMe
{
    public int A() => 42;

    public int B() => 42;

    private int A() => 0815;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_2_alternating_public_and_private_methods_sharing_same_name() => No_issue_is_reported_for(@"
public class TestMe
{
    public int A() => 42;

    public int B() => 42;

    private int A() => 0815;

    private int B() => 0815;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_static_and_non_static_methods_sharing_same_name_and_1_method_in_between() => No_issue_is_reported_for(@"
public class TestMe
{
    private static int A() => 0815;

    private int B() => 4711;

    private int A(object o) => 0815;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_private_static_and_non_static_methods_sharing_same_name_and_non_static_methods_with_same_names_not_side_by_side() => An_issue_is_reported_for(@"
public class TestMe
{
    private static int A() => 0815;

    private int B() => 4711;

    private int A(object o) => 0815;

    private int B(object o) => 0815;
}
");

        protected override string GetDiagnosticId() => MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer();
    }
}