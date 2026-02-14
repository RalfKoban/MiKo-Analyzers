using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2015_FireMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class_method_property_event_and_field_without_documentation() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_class_method_property_event_and_field_with_documentation_not_containing_fire_terms() => No_issue_is_reported_for(@"
using System;

/// <summary>Does something.</summary>
/// <remarks>Does something.</remarks>
public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    public event EventHandler<T> MyEvent;

    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    public void DoSomething() { }

    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    public int Age { get; set; }

    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    private bool m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_class_documentation_containing_fire_term_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

/// <" + tag + ">Does fire.</" + tag + @">
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_method_documentation_containing_fire_term_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Does fire.</" + tag + @">
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_property_documentation_containing_fire_term_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Does fire.</" + tag + @">
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_event_documentation_containing_fire_term_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Does fire.</" + tag + @">
    public event EventHandler<T> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_field_documentation_containing_fire_term_in_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Does fire.</" + tag + @">
    private bool m_field;
}
");

        [Test]
        public void Code_gets_fixed_to_replace_fire_terms_with_throw_for_exception_documentation_on_single_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Fires a new exception. The code is firing or will fire the fired exception.</summary>
    public void DoSomething() { }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Throws a new exception. The code is throwing or will throw the thrown exception.</summary>
    public void DoSomething() { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_replace_fire_terms_with_throw_for_exception_documentation_on_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Fires a new exception.
    /// The code is firing or will fire the fired exception.
    /// </summary>
    public void DoSomething() { }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Throws a new exception.
    /// The code is throwing or will throw the thrown exception.
    /// </summary>
    public void DoSomething() { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_replace_fire_terms_with_raise_for_event_documentation_on_single_line()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Fires a new event. The code is firing or will fire the fired event.</summary>
    public void DoSomething() { }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Raises a new event. The code is raising or will raise the raised event.</summary>
    public void DoSomething() { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_replace_fire_terms_with_raise_for_event_documentation_on_multiple_lines()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Fires a new event.
    /// The code is firing or will fire the fired event.
    /// </summary>
    public void DoSomething() { }
}";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Raises a new event.
    /// The code is raising or will raise the raised event.
    /// </summary>
    public void DoSomething() { }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2015_FireMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2015_FireMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2015_CodeFixProvider();
    }
}