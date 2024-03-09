using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2078_CodeShouldNotUseXmlTagsAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_documented_items_without_code() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_code_without_Xml_on_type() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// <code>
/// var x = 42;
/// </code>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_code_without_Xml_on_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_code_without_Xml_on_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_code_without_Xml_on_event() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// var x = 42;
    /// </code>
    /// </summary>
    public event EventHandler<T> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_code_with_Xml_on_type() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// <code>
/// <some></some>
/// </code>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_code_with_Xml_on_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// <some></some>
    /// </code>
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_code_with_Xml_on_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// <some></some>
    /// </code>
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_code_with_Xml_on_event() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// <some></some>
    /// </code>
    /// </summary>
    public event EventHandler<T> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_code_with_empty_Xml_on_type() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Some documentation.
/// <code>
/// <some />
/// </code>
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_code_with_empty_Xml_on_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// <some />
    /// </code>
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_code_with_empty_Xml_on_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// <some />
    /// </code>
    /// </summary>
    public int Age { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_code_with_empty_Xml_on_event() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some documentation.
    /// <code>
    /// <some />
    /// </code>
    /// </summary>
    public event EventHandler<T> MyEvent;
}
");

        protected override string GetDiagnosticId() => MiKo_2078_CodeShouldNotUseXmlTagsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2078_CodeShouldNotUseXmlTagsAnalyzer();
    }
}