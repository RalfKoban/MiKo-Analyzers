using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public class MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_incomplete_ctor__when_colon_is_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() :
}
");

        [Test]
        public void No_issue_is_reported_for_incomplete_ctor__when_colon_is_on_other_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe()
                :
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_calling_other_this_ctor_when_colon_is_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() : this(42) {}

    public TestMe(int i) {}
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_calling_other_base_ctor_when_colon_is_on_same_line() => No_issue_is_reported_for(@"
public class TestMeBase
{
    public TestMeBase(int i) {}
}
public class TestMe : TestMeBase
{
    public TestMe() : base(42) {}
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_calling_other_this_ctor_when_colon_is_on_same_line_but_below_ctor() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe()
                : this(42) {}

    public TestMe(int i) {}
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_calling_other_base_ctor_when_colon_is_on_same_line_but_below_ctor() => No_issue_is_reported_for(@"
public class TestMeBase
{
    public TestMeBase(int i) {}
}
public class TestMe : TestMeBase
{
    public TestMe()
                : base(42) {}
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_calling_other_this_ctor_when_colon_is_on_other_line_than_invoked_ctor() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() :
                this(42) {}

    public TestMe(int i) {}
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_calling_other_base_ctor_when_colon_is_on_other_line_than_invoked_ctor() => An_issue_is_reported_for(@"
public class TestMeBase
{
    public TestMeBase(int i) {}
}
public class TestMe : TestMeBase
{
    public TestMe() :
                base(42) {}
}
");

        [Test]
        public void Code_gets_fixed_for_ctor_calling_other_this_ctor_when_colon_is_on_other_line_than_invoked_ctor()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe() :
                this(42) {}

    public TestMe(int i) {}
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe()
                : this(42) {}

    public TestMe(int i) {}
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ctor_calling_other_base_ctor_when_colon_is_on_other_line_than_invoked_ctor()
        {
            const string OriginalCode = @"
public class TestMeBase
{
    public TestMeBase(int i) {}
}

public class TestMe : TestMeBase
{
    public TestMe() :
                base(42) {}
}
";

            const string FixedCode = @"
public class TestMeBase
{
    public TestMeBase(int i) {}
}

public class TestMe : TestMeBase
{
    public TestMe()
                : base(42) {}
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6051_CodeFixProvider();
    }
}