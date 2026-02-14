using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2056_ObjectDisposedExceptionPhraseAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_documented_method_without_exception_documentation() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_documented_method_with_other_exception_type() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_ObjectDisposedException_with_standard_disposed_phrase() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
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
        public void No_issue_is_reported_for_ObjectDisposedException_with_standard_disposed_phrase_inside_para_tag() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">
    /// <para>
    /// The object has been disposed.
    /// </para>
    /// </exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectDisposedException_with_non_standard_phrase() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">Object gone.</exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ObjectDisposedException_with_non_standard_phrase_inside_para_tags() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException""><para>Object gone.</para></exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ObjectDisposedException_with_standard_closed_phrase_when_Close_method_exists() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Close()
    {
    }

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
        public void No_issue_is_reported_for_ObjectDisposedException_with_standard_closed_phrase_when_Close_method_exists_in_base_class() => No_issue_is_reported_for(@"
using System;

public class Base
{
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
        public void An_issue_is_reported_for_ObjectDisposedException_with_non_standard_phrase_when_Close_method_exists() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Close()
    {
    }

    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">Object gone.</exception>
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_disposed_phrase_for_fully_qualified_exception()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""System.ObjectDisposedException"">The object.</exception>
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
    /// <exception cref=""System.ObjectDisposedException"">The object has been disposed.</exception>
    public void DoSomething()
    {
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_disposed_phrase()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">The object.</exception>
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
    /// <exception cref=""ObjectDisposedException"">The object has been disposed.</exception>
    public void DoSomething()
    {
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_replacing_with_standard_closed_phrase_when_Close_method_exists()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public void Close()
    {
    }

    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">The object.</exception>
    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public void Close()
    {
    }

    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <exception cref=""ObjectDisposedException"">The object has been closed.</exception>
    public void DoSomething()
    {
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2056_ObjectDisposedExceptionPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2056_ObjectDisposedExceptionPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2056_CodeFixProvider();
    }
}