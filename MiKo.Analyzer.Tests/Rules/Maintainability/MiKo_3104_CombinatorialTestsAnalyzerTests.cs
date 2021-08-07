using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3104_CombinatorialTestsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_method_having_no_Combinatorial_attribute_and_no_parameter() => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_method_having_no_Combinatorial_attribute_and_a_single_parameter() => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething(bool b)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_method_having_Sequential_attribute_and_at_least_2_parameters() => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Sequential]
    public void DoSomething([Values] bool b, [Range(0, 1, 2)] int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_method_having_Pairwise_attribute_and_at_least_2_parameters() => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Pairwise]
    public void DoSomething([Values] bool b, [Range(0, 1, 2)] int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_method_having_no_Combinatorial_attribute_but_at_least_2_parameters() => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething([Values] bool b, [Range(0, 1, 2)] int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_method_having_a_Combinatorial_attribute_in_a_combined_AttributeList_but_no_parameters() => An_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Combinatorial]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_method_having_a_Combinatorial_attribute_in_a_combined_AttributeList_but_only_a_single_parameter() => An_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Combinatorial]
    public void DoSomething(bool b)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_method_having_a_Combinatorial_attribute_and_a_Sequential_attribute_in_a_combined_AttributeList() => An_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Combinatorial, Sequential]
    public void DoSomething(bool b)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_method_having_a_Combinatorial_attribute_and_a_Pairwise_attribute_in_a_combined_AttributeList() => An_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Combinatorial, Pairwise]
    public void DoSomething(bool b)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_unneeded_Combinatorial_attribute_on_same_line()
        {
            const string OriginalCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Combinatorial]
    public void DoSomething(bool b)
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething(bool b)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unneeded_Combinatorial_attribute_on_different_lines()
        {
            const string OriginalCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    [Combinatorial]
    public void DoSomething(bool b)
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething(bool b)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_combined_Combinatorial_and_Sequential_attribute()
        {
            const string OriginalCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Combinatorial, Sequential]
    public void DoSomething([Values] bool a, [Values] bool b)
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Sequential]
    public void DoSomething([Values] bool a, [Values] bool b)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_combined_Combinatorial_and_Pairwise_attribute()
        {
            const string OriginalCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Combinatorial, Pairwise]
    public void DoSomething([Values] bool a, [Values] bool b)
    {
    }
}
";

            const string FixedCode = @"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test, Pairwise]
    public void DoSomething([Values] bool a, [Values] bool b)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3104_CombinatorialTestsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3104_CombinatorialTestsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3104_CodeFixProvider();
    }
}