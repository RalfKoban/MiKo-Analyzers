using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2015_FireMethodsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = { "summary", "remarks", "returns", "example", "value", "exception" };

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
        public void No_issue_is_reported_for_correctly_documented_items() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_documented_class_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

/// <" + tag + ">Does fire.</" + tag + @">
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Does fire.</" + tag + @">
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_property_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Does fire.</" + tag + @">
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_event_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Does fire.</" + tag + @">
    public event EventHandler<T> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_field_([ValueSource(nameof(XmlTags))] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Does fire.</" + tag + @">
    private bool m_field;
}
");

        [Test]
        public void Code_gets_fixed_for_exception()
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
        public void Code_gets_fixed_for_exception_when_on_different_lines()
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
        public void Code_gets_fixed_for_event()
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
        public void Code_gets_fixed_for_event_when_on_different_lines()
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