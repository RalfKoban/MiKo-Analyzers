using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2049_WillBePhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = { "summary", "remarks", "returns", "example", "value", "exception" };

        private static readonly string[] Phrases =
                                                   {
                                                       "It will be.",
                                                       "It will also be.",
                                                       "It will as well be.",
                                                       "It will return.",
                                                       "It is something (will leave something)",
                                                       "It will not be something.",
                                                       "It will not contain.",
                                                       "It will never be something.",
                                                       "It will never return.",
                                                   };

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
        public void An_issue_is_reported_for_incorrectly_documented_class_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

/// <" + tag + ">" + phrase + "</" + tag + @">
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_method_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">" + phrase + "</" + tag + @">
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_property_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">" + phrase + "</" + tag + @">
    public int Age { get; set; }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_event_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">" + phrase + "</" + tag + @">
    public event EventHandler<T> MyEvent;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_field_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <" + tag + ">" + phrase + "</" + tag + @">
    private bool m_field;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_interface_property_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public interface ITestMe
{
    /// <" + tag + ">" + phrase + "</" + tag + @">
    int Age { get; set; }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_documented_interface_indexer_([ValueSource(nameof(XmlTags))] string tag, [ValueSource(nameof(Phrases))] string phrase) => An_issue_is_reported_for(@"
using System;

public interface ITestMe
{
    /// <" + tag + ">" + phrase + "</" + tag + @">
    int this[int key] { get; set; }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_gets_reported() => An_issue_is_reported_for(3, @"
using System;

public interface ITestMe
{
    /// <summary>
    /// The something will not check for something when the next something will be opened. Therefore, there will be no stuff.
    /// </summary>
    int DoSomething()
}
");

        [TestCase("It will be something.", "It is something.")]
        [TestCase("It will also be something.", "It is something.")]
        [TestCase("It will as well be something.", "It is something.")]
        [TestCase("It will return.", "It returns.")]
        [TestCase("It is something (will leave something)", "It is something (leaves something)")]
        [TestCase("It will not be something.", "It is not something.")]
        [TestCase("It will not contain something.", "It does not contain something.")]
        [TestCase("It will never be something.", "It is never something.")]
        [TestCase("It will never return.", "It never returns.")]
        [TestCase("Will return.", "Returns.")]
        [TestCase("Will never return.", "Never returns.")]
        [TestCase("It will automatically return.", "It automatically returns.")]
        [TestCase("It will randomly return.", "It randomly returns.")]
        [TestCase("It will apply something to it.", "It applies something to it.")]
        public void Code_gets_fixed_(string originalPhrase, string fixedPhrase)
        {
            const string Template = @"
using System;

public interface ITestMe
{
    /// <summary>###</summary>
    int DoSomething()
}
";

            VerifyCSharpFix(Template.Replace("###", originalPhrase), Template.Replace("###", fixedPhrase));
        }

        protected override string GetDiagnosticId() => MiKo_2049_WillBePhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2049_WillBePhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2049_CodeFixProvider();
    }
}