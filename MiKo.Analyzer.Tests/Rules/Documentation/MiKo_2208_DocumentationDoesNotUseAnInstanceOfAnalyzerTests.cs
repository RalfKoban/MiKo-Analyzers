using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Phrases =
                                                   {
                                                       "An instance of ",
                                                       "A instance of ",
                                                       "The instance of ",
                                                       "An object of ",
                                                       "A object of ",
                                                       "The object of ",
                                                       "an instance of ",
                                                       "a instance of ",
                                                       "the instance of ",
                                                       "an object of ",
                                                       "a object of ",
                                                       "the object of ",
                                                       "An instance if ", // 'semi'-typo by intent
                                                       "A instance if ", // 'semi'-typo by intent
                                                       "The instance if ", // 'semi'-typo by intent
                                                       "an instance if ", // 'semi'-typo by intent
                                                       "a instance if ", // 'semi'-typo by intent
                                                       "the instance if ", // 'semi'-typo by intent
                                                   };

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

        [Test, Combinatorial]
        public void No_issue_is_reported_for_code_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => No_issue_is_reported_for(@"
using System;

/// <" + tag + "><code>" + phrase + "something.</code></" + tag + @">
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_class_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

/// <" + tag + ">" + phrase + "something.</" + tag + @">
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_method_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Handles " + phrase + "something.</" + tag + @">
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_property_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Handles " + phrase + "something.</" + tag + @">
    public int Age { get; set; }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_event_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Handles " + phrase + "something.</" + tag + @">
    public event EventHandler<T> MyEvent;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_field_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">Handles " + phrase + "something.</" + tag + @">
    private bool m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_factory_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Creates an instance of the <see cref=""TestMe"" /> type.
    /// </summary>
    public TestMe DoSomething() { }
}
");

        [Test]
        public void Code_gets_fixed_for_([ValueSource(nameof(Phrases))] string text)
        {
            const string Template = @"
using System;

public class TestMe
{
    /// <summary>
    /// ### something.
    /// </summary>
    public TestMe DoSomething() { }
}
";

            VerifyCSharpFix(Template.Replace("###", text + "an"), Template.Replace("###", text[0].IsUpperCase() ? "An" : "an"));
            VerifyCSharpFix(Template.Replace("###", text.Trim()), Template.Replace("###", text.FirstWord()));
        }

        protected override string GetDiagnosticId() => MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2208_CodeFixProvider();
    }
}