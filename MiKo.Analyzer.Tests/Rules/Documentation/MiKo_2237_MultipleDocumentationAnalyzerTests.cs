using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2237_MultipleDocumentationAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = ["summary", "remarks"];

        [Test]
        public void No_issue_is_reported_for_undocumented_items() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler<T> MyEvent;

    public void DoSomething() { }

    public int Age { get; set; }

    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_items_with_multiple_elements() => No_issue_is_reported_for(@"
using System;

/// <summary>This is some text.</summary>
/// <remarks>This is some text.</remarks>
public class TestMe
{
    /// <summary>This is some text.</summary>
    /// <remarks>This is some text.</remarks>
    public event EventHandler<T> MyEvent;

    /// <summary>This is some text.</summary>
    /// <remarks>This is some text.</remarks>
    public void DoSomething() { }

    /// <summary>This is some text.</summary>
    /// <remarks>This is some text.</remarks>
    public int Age { get; set; }

    /// <summary>This is some text.</summary>
    /// <remarks>This is some text.</remarks>
    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_items_with_single_elements() => No_issue_is_reported_for(@"
using System;

/// <summary>This is some text.</summary>
public class TestMe
{
    /// <summary>This is some text.</summary>
    public event EventHandler<T> MyEvent;

    /// <summary>This is some text.</summary>
    public void DoSomething() { }

    /// <summary>This is some text.</summary>
    public int Age { get; set; }

    /// <summary>This is some text.</summary>
    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_commented_out_code_with_documentation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>This is some text.</summary>
    // private bool m_field;

    /// <summary>This is some text.</summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_commented_out_single_const_field_with_documentation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text for commented out field.
    /// </summary>
    // private const MyField1 = ""42"";

    /// <summary>
    /// This is some text for field 2.
    /// </summary>
    private const MyField2 = ""42"";
}
");

        [Test]
        public void No_issue_is_reported_for_commented_out_const_field_with_documentation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// This is some text for field 1. It is important to have this field here!
    /// </summary>
    private const MyField1 = ""42"";

    /// <summary>
    /// This is some text for commented out field 2.
    /// </summary>
    // private const MyField2 = ""42"";

    /// <summary>
    /// This is some text for field 3.
    /// </summary>
    private const MyField3 = ""42"";
}
");

        [Test]
        public void An_issue_is_reported_for_gaps_between_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

/// <" + tag + ">This is some text before the gap.</" + tag + @">

/// <" + tag + ">This is some text after the gap.</" + tag + @">
public class TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_for_gaps_between_summaries()
        {
            const string OriginalCode = @"
using System;

/// <summary>This is some text before the gap.</summary>

/// <summary>This is some text after the gap.</summary>
public class TestMe
{
}
";

            const string FixedCode = @"
using System;

/// <summary>This is some text before the gap.</summary>
/// <summary>This is some text after the gap.</summary>
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2237_MultipleDocumentationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2237_MultipleDocumentationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2237_CodeFixProvider();
    }
}