using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2057_ObjectDisposedExceptionWrongPlacedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_without_exception_docu() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_with_exception_docu_for_other_exception() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ArgumentException"">Some text</exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
    public void Dispose() { }

    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">The object has been disposed.</exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException""></exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_with_Dispose_in_base_class() => No_issue_is_reported_for(@"
using System;

public class Base : IDisposable
{
    public void Dispose() { }

    public void Close()
    {
    }
}

public class TestMe : Base
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">The object has been closed.</exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException""></exception>
    /// <exception cref=""ArgumentNullException""></exception>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ArgumentNullException""></exception>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2057_ObjectDisposedExceptionWrongPlacedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2057_ObjectDisposedExceptionWrongPlacedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2057_CodeFixProvider();
    }
}