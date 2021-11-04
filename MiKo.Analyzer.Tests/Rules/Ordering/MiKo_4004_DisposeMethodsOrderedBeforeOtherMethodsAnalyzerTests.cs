using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4004_DisposeMethodsOrderedBeforeOtherMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_without_Dispose_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void A() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_public_Dispose_method_as_first_method_if_no_ctor_or_finalizer_is_available() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_if_no_ctor_or_finalizer_is_available() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_public_Dispose_method_as_first_method_after_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_after_ctor() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_public_Dispose_method_as_first_method_after_finalizer() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    ~TestMe() { }

    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_as_first_method_after_finalizer() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    ~TestMe() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_public_Dispose_method_before_other_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    public void Dispose() { }

    public void A() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_public_Dispose_method_after_public_static_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static void A() { }

    public void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_interface_method_before_other_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    ~TestMe() { }

    void IDisposable.Dispose() { }

    public void A() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_protected_Dispose_method_after_all_public_methods_and_before_other_protected_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    protected void Dispose() { }

    protected void B() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_protected_Dispose_method_after_all_public_methods_and_before_private_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    protected void Dispose() { }

    private void B() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_Dispose_method_after_all_public_and_protected_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    private void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_Dispose_method_after_all_public_and_protected_methods_and_before_other_private_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    private void Dispose() { }

    private void C() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_Dispose_method_after_all_public_and_protected_and_private_static_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    private static void C() { }

    private void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_Dispose_method_after_all_public_and_protected_and_private_static_methods_and_before_other_private_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    private static void C() { }

    private void Dispose() { }

    private void D() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_interface_method_after_other_public_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_public_Dispose_method_after_other_public_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    public void Dispose() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_protected_Dispose_method_after_other_protected_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    protected void Dispose() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_private_Dispose_method_after_other_private_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    private void C() { }

    private void Dispose() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_methods_as_only_matching_methods_with_same_accessibility_after_all_other_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static void A() { }

    protected static void B() { }

    private void C() { }

    public void Dispose() { }

    protected void Dispose(bool disposing) { }
}
");

        //// TODO RKN: partial parts

        [Test]
        public void Code_gets_fixed_for_class_with_Dispose_interface_method_after_other_public_methods()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public void A() { }

    void IDisposable.Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    void IDisposable.Dispose() { }

    public void A() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_public_Dispose_method_after_other_public_methods()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void A() { }

    public void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void Dispose() { }

    public void A() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_protected_Dispose_method_after_other_protected_methods()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    protected void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void A() { }

    protected void Dispose() { }

    protected void B() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_private_Dispose_method_after_other_private_methods()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    private void C() { }

    private void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void A() { }

    protected void B() { }

    private void Dispose() { }

    private void C() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4004_DisposeMethodsOrderedBeforeOtherMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4004_DisposeMethodsOrderedBeforeOtherMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4004_CodeFixProvider();
    }
}