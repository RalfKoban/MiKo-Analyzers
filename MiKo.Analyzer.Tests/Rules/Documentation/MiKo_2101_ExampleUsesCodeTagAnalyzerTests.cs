using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2101_ExampleUsesCodeTagAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_items() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler<T> MyEvent;

    public void DoSomething() { }

    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_items_without_example() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// </summary>
public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    public event EventHandler<T> MyEvent;

    /// <summary>
    /// Some documentation.
    /// </summary>
    public void DoSomething() { }

    /// <summary>
    /// Some documentation.
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_example_with_code_tag() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// </summary>
/// <example>
/// The following example demonstrates its usage.
/// <code>
/// var x = 42;
/// </code>
/// </example>
public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// The following example demonstrates its usage.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </example>
    public event EventHandler<T> MyEvent;

    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// The following example demonstrates its usage.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </example>
    public void DoSomething() { }

    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// The following example demonstrates its usage.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </example>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_example_without_code_tag_on_type() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// </summary>
/// <example>
/// var x = 42;
/// </example>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_example_without_code_tag_on_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// var x = 42;
    /// </example>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_example_without_code_tag_on_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// var x = 42;
    /// </example>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_example_without_code_tag_on_event() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// var x = 42;
    /// </example>
    public event EventHandler<T> MyEvent;
}
");

        [Test]
        public void Code_gets_fixed_for_single_line_example_on_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// var x = 42;
    /// </example>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// <code>
    /// var x = 42;
    /// </code>
    /// </example>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multi_line_example_on_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// var x = 42;
    /// var y = x / 2;
    /// </example>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// <code>
    /// var x = 42;
    /// var y = x / 2;
    /// </code>
    /// </example>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_example_with_preceding_text_on_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// Here some example:
    /// var x = 42;
    /// var y = x / 2;
    /// </example>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// Here some example:
    /// <code>
    /// var x = 42;
    /// var y = x / 2;
    /// </code>
    /// </example>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_example_with_surrounding_text_on_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// Here some example:
    /// var x = 42;
    /// var y = x / 2;
    /// As you can see, there is something.
    /// </example>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// </summary>
    /// <example>
    /// Here some example:
    /// <code>
    /// var x = 42;
    /// var y = x / 2;
    /// </code>
    /// As you can see, there is something.
    /// </example>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2101_ExampleUsesCodeTagAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2101_ExampleUsesCodeTagAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2101_CodeFixProvider();
    }
}