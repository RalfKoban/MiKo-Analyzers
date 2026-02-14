using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4105_ObjectUnderTestFieldOrderingAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_class_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_only_a_Test_method_(
                                                                               [ValueSource(nameof(TestFixtures))] string fixture,
                                                                               [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_the_field_as_the_only_field_(
                                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
}

[" + fixture + @"]
public class TestMeTests
{
    private TestMe _objectUnderTest;

    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_the_field_as_very_first_field_(
                                                                                          [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                          [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
}

[" + fixture + @"]
public class TestMeTests
{
    private TestMe _objectUnderTest;
    private int _someField;

    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_the_field_coming_after_constant_fields_(
                                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                   [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
}

[" + fixture + @"]
public class TestMeTests
{
    private const int SomeId = 42;
    private TestMe _objectUnderTest;
    private int _someField;

    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_test_class_with_the_field_coming_after_only_constant_fields_(
                                                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
}

[" + fixture + @"]
public class TestMeTests
{
    private const int SomeId = 42;
    private TestMe _objectUnderTest;

    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_class_with_the_field_as_non_first_field_(
                                                                                         [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                         [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
}

[" + fixture + @"]
public class TestMeTests
{
    private int _someField;
    private TestMe _objectUnderTest;

    [" + test + @"]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_field_as_last_field()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    private int _someField1;
    private int _someField2;
    private TestMe _objectUnderTest;

    [Test]
    public void DoSomething()
    {
    }
}";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    private TestMe _objectUnderTest;
    private int _someField1;
    private int _someField2;

    [Test]
    public void DoSomething()
    {
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_as_middle_field()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    private int _someField1;
    private TestMe _objectUnderTest;
    private int _someField2;

    [Test]
    public void DoSomething()
    {
    }
}";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    private TestMe _objectUnderTest;
    private int _someField1;
    private int _someField2;

    [Test]
    public void DoSomething()
    {
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_as_last_field_if_all_fields_are_at_end_of_type()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    [Test]
    public void DoSomething()
    {
    }

    private int _someField1;
    private int _someField2;
    private TestMe _objectUnderTest;
}";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    [Test]
    public void DoSomething()
    {
    }

    private TestMe _objectUnderTest;
    private int _someField1;
    private int _someField2;
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_as_middle_field_if_all_fields_are_at_end_of_type()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    [Test]
    public void DoSomething()
    {
    }

    private int _someField1;
    private TestMe _objectUnderTest;
    private int _someField2;
}";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    [Test]
    public void DoSomething()
    {
    }

    private TestMe _objectUnderTest;
    private int _someField1;
    private int _someField2;
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_as_middle_field_with_additional_constant_fields()
        {
            const string OriginalCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    private const int SomeId = 42;
    private int _someField1;
    private TestMe _objectUnderTest;
    private int _someField2;

    [Test]
    public void DoSomething()
    {
    }
}";

            const string FixedCode = @"
using NUnit.Framework;

public class TestMe
{
}

[TestFixture]
public class TestMeTests
{
    private const int SomeId = 42;
    private TestMe _objectUnderTest;
    private int _someField1;
    private int _someField2;

    [Test]
    public void DoSomething()
    {
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4105_ObjectUnderTestFieldOrderingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4105_ObjectUnderTestFieldOrderingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4105_CodeFixProvider();
    }
}