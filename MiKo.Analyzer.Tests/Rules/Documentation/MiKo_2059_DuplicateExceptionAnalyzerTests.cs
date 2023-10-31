using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2059_DuplicateExceptionAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_documented_method_with_docu_for_single_exception() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_documented_method_with_docu_for_different_exceptions() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ArgumentException"">Some text</exception>
    /// <exception cref=""ArgumentOutOfRangeException"">Some other text</exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_documented_method_with_multiple_docu_for_same_exception() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ArgumentException"">Some text</exception>
    /// <exception cref=""ArgumentOutOfRangeException"">Some other text</exception>
    /// <exception cref=""ArgumentException"">Some more text</exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_single_duplicate_exception()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ArgumentException"">Some text</exception>
    /// <exception cref=""ArgumentOutOfRangeException"">Some other text</exception>
    /// <exception cref=""ArgumentException"">Some more text</exception>
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
    /// <exception cref=""ArgumentException"">
    /// Some text
    /// <para>-or-</para>
    /// Some more text
    /// </exception>
    /// <exception cref=""ArgumentOutOfRangeException"">Some other text</exception>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_duplicate_exceptions()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ArgumentException"">Some text</exception>
    /// <exception cref=""ArgumentOutOfRangeException"">Some other text</exception>
    /// <exception cref=""ArgumentException"">Some more text</exception>
    /// <exception cref=""InvalidOperationException"">Something wrong.</exception>
    /// <exception cref=""ArgumentException"">Some even more text</exception>
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
    /// <exception cref=""ArgumentException"">
    /// Some text
    /// <para>-or-</para>
    /// Some more text
    /// <para>-or-</para>
    /// Some even more text
    /// </exception>
    /// <exception cref=""ArgumentOutOfRangeException"">Some other text</exception>
    /// <exception cref=""InvalidOperationException"">Something wrong.</exception>
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2059_DuplicateExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2059_DuplicateExceptionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2059_CodeFixProvider();
    }
}