﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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

public class TestMe : IDisposable
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

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void Dispose(bool disposing) { }

    protected void B() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_protected_Dispose_method_after_all_public_methods_and_before_private_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void Dispose(bool disposing) { }

    private void B() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_Dispose_method_after_all_public_and_protected_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_Dispose_method_after_all_public_and_protected_methods_and_before_other_private_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    private void Dispose(bool disposing) { }

    private void C() { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_Dispose_method_after_all_public_and_protected_and_private_static_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    private static void C() { }

    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_Dispose_method_after_all_public_and_protected_and_private_static_methods_and_before_other_private_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    private static void C() { }

    private void Dispose(bool disposing) { }

    private void D() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_Dispose_interface_method_after_other_public_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void A() { }

    void IDisposable.Dispose() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_public_Dispose_method_after_other_public_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void A() { }

    public void Dispose() { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_protected_Dispose_method_after_other_protected_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    protected void Dispose(bool disposing) { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_private_Dispose_method_after_other_private_methods() => An_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    private void C() { }

    private void Dispose(bool disposing) { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_Dispose_methods_as_only_matching_methods_with_same_accessibility_after_all_other_methods() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public static void A() { }

    protected static void B() { }

    private void C() { }

    public void Dispose() { }

    protected void Dispose(bool disposing) { }
}
");

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

public class TestMe : IDisposable
{
    public void A() { }

    public void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
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

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    protected void Dispose(bool disposing) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void Dispose(bool disposing) { }

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

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    private void C() { }

    private void Dispose(bool disposing) { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    private void Dispose(bool disposing) { }

    private void C() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_public_Dispose_method_after_other_methods_and_properties()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public string Name { get; set; }

    public event EventHandler MyEvent;

    public static void DoSomething() { }

    public void A() { }

    public void Dispose() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public string Name { get; set; }

    public event EventHandler MyEvent;

    public static void DoSomething() { }

    public void Dispose() { }

    public void A() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_Dispose_interface_method_after_other_public_methods_when_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public void A() { }

    #region Disposal

    void IDisposable.Dispose() { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    #region Disposal

    void IDisposable.Dispose() { }

    #endregion

    public void A() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_public_Dispose_method_after_other_public_methods_when_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public void A() { }

    #region Disposal

    public void Dispose() { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    #region Disposal

    public void Dispose() { }

    #endregion

    public void A() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_protected_Dispose_method_after_other_protected_methods_when_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    #region Disposal

    protected void Dispose(bool disposing) { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    #region Disposal

    protected void Dispose(bool disposing) { }

    #endregion

    protected void B() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_protected_Dispose_method_after_other_protected_methods_when_both_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    #region Protected methods

    protected void B() { }

    protected void Dispose(bool disposing) { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    #region Protected methods

    protected void Dispose(bool disposing) { }

    protected void B() { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_private_Dispose_method_after_other_private_methods_when_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    private void C() { }

    #region Disposal

    private void Dispose(bool disposing) { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    #region Disposal

    private void Dispose(bool disposing) { }

    #endregion

    private void C() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_private_Dispose_method_after_other_private_methods_when_both_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    #region Private methods

    private void C() { }

    private void Dispose(bool disposing) { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    public void A() { }

    protected void B() { }

    #region Private methods

    private void Dispose(bool disposing) { }

    private void C() { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_public_Dispose_method_after_other_methods_and_properties_when_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public string Name { get; set; }

    public event EventHandler MyEvent;

    public static void DoSomething() { }

    public void A() { }

    #region Disposal

    public void Dispose() { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public string Name { get; set; }

    public event EventHandler MyEvent;

    public static void DoSomething() { }

    #region Disposal

    public void Dispose() { }

    #endregion

    public void A() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_public_Dispose_method_after_other_methods_and_properties_when_both_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public string Name { get; set; }

    public event EventHandler MyEvent;

    public static void DoSomething() { }

    #region Public methods

    public void A() { }

    public void Dispose() { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public string Name { get; set; }

    public event EventHandler MyEvent;

    public static void DoSomething() { }

    #region Public methods

    public void Dispose() { }

    public void A() { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_public_Dispose_method_after_other_methods_and_properties_when_all_public_put_in_region()
        {
            const string OriginalCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public string Name { get; set; }

    public event EventHandler MyEvent;

    #region Public methods

    public static void DoSomething() { }

    public void A() { }

    public void Dispose() { }

    #endregion
}
";

            const string FixedCode = @"
using System;

public class TestMe : IDisposable
{
    public TestMe() { }

    public string Name { get; set; }

    public event EventHandler MyEvent;

    #region Public methods

    public static void DoSomething() { }

    public void Dispose() { }

    public void A() { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        //// TODO RKN: partial parts

        protected override string GetDiagnosticId() => MiKo_4004_DisposeMethodsOrderedBeforeOtherMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4004_DisposeMethodsOrderedBeforeOtherMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4004_CodeFixProvider();
    }
}