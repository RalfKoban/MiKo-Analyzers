using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4008_GetHashCodeMethodsOrderedAfterEqualsMethodAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_without_GetHashCode_override() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_override_but_no_Equals_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public override int GetHashCode() => 42;

    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_override_placed_after_Equals_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public override int GetHashCode() => 42;

    public void DoSomethingElse() { }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_override_placed_between_multiple_Equals_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IEquatable<TestMe>
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public override int GetHashCode() => 42;

    public bool Equals(TestMe other) => false;

    public void DoSomethingElse() { }
}
");

        [Test]
        public void No_issue_is_reported_for_GetHashCode_override_placed_before_non_public_Equals_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override int GetHashCode() => 42;

    public void DoSomethingElse() { }

    private bool Equals(object o, bool flag) => false;
}
");

        [Test]
        public void An_issue_is_reported_for_GetHashCode_override_placed_before_Equals_method() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override int GetHashCode() => 42;

    public override bool Equals(object o) => false;

    public void DoSomethingElse() { }
}
");

        [Test]
        public void An_issue_is_reported_for_GetHashCode_override_placed_after_other_than_Equals_method() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public void DoSomethingElse() { }

    public override int GetHashCode() => 42;
}
");

        [Test]
        public void Code_gets_fixed_for_GetHashCode_override_placed_before_Equals_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override int GetHashCode() => 42;

    public override bool Equals(object o) => false;

    public void DoSomethingElse() { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public override int GetHashCode() => 42;

    public void DoSomethingElse() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_override_placed_after_other_than_Equals_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public void DoSomethingElse() { }

    public override int GetHashCode() => 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public override int GetHashCode() => 42;

    public void DoSomethingElse() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_override_placed_after_other_than_any_of_multiple_Equals_methods_when_override_Equals_comes_first()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IEquatable<TestMe>
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public bool Equals(TestMe other) => false;

    public void DoSomethingElse() { }

    public override int GetHashCode() => 42;
}
";

            const string FixedCode = @"
using System;

public class TestMe : IEquatable<TestMe>
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public bool Equals(TestMe other) => false;

    public override int GetHashCode() => 42;

    public void DoSomethingElse() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_override_placed_after_other_than_any_of_multiple_Equals_methods_when_override_Equals_comes_last()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IEquatable<TestMe>
{
    public TestMe() { }

    public void DoSomething() { }

    public bool Equals(TestMe other) => false;

    public override bool Equals(object o) => false;

    public void DoSomethingElse() { }

    public override int GetHashCode() => 42;
}
";

            const string FixedCode = @"
using System;

public class TestMe : IEquatable<TestMe>
{
    public TestMe() { }

    public void DoSomething() { }

    public bool Equals(TestMe other) => false;

    public override bool Equals(object o) => false;

    public override int GetHashCode() => 42;

    public void DoSomethingElse() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_GetHashCode_override_placed_after_public_Equals_methods_when_a_private_Equals_exists()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public void DoSomethingElse() { }

    public override int GetHashCode() => 42;

    private bool Equals(TestMe other) => false;
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe() { }

    public void DoSomething() { }

    public override bool Equals(object o) => false;

    public override int GetHashCode() => 42;

    public void DoSomethingElse() { }

    private bool Equals(TestMe other) => false;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4008_GetHashCodeMethodsOrderedAfterEqualsMethodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4008_GetHashCodeMethodsOrderedAfterEqualsMethodAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4008_CodeFixProvider();
    }
}