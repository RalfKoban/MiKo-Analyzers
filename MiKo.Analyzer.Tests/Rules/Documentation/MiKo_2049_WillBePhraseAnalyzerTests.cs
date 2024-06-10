using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2049_WillBePhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = ["summary", "remarks", "returns", "example", "value", "exception"];

        private static readonly string[] Phrases =
                                                   [
                                                       "It will be.",
                                                       "It will also be.",
                                                       "It will as well be.",
                                                       "It will return.",
                                                       "It is something (will leave something)",
                                                       "It will not be something.",
                                                       "It will not contain.",
                                                       "It will never be something.",
                                                       "It will never return.",
                                                   ];

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

        [TestCase("Something not willing to do anything.")]
        public void No_issue_is_reported_for_specific_comment_(string phrase) => No_issue_is_reported_for(@"
using System;

/// <summary>" + phrase + @"</summary>
public class TestMe
{
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
        [TestCase("It is something (will exist).", "It is something (exists).")]
        [TestCase("It will not be something.", "It is not something.")]
        [TestCase("It will not contain something.", "It does not contain something.")]
        [TestCase("It will never be something.", "It is never something.")]
        [TestCase("It will never return.", "It never returns.")]
        [TestCase("Will return.", "Returns.")]
        [TestCase("Will never return.", "Never returns.")]
        [TestCase("It will automatically return.", "It automatically returns.")]
        [TestCase("It will randomly return.", "It randomly returns.")]
        [TestCase("It will apply something to it.", "It applies something to it.")]
        [TestCase("Something will added to something else.", "Something adds to something else.")]
        [TestCase("The object that the search will base on.", "The object that the search is based on.")]
        [TestCase("The object will also get notified.", "The object also gets notified.")]
        [TestCase("The object will just get notified.", "The object just gets notified.")]
        [TestCase("The object will then get notified.", "The object then gets notified.")]
        [TestCase("The object will than get notified.", "The object than gets notified.")] // typo 'than' instead of 'then'
        [TestCase("The object will therefore get notified.", "The object therefore gets notified.")]
        [TestCase("The object will therefore not get notified.", "The object therefore not gets notified.")]
        [TestCase("The object will however get notified.", "The object however gets notified.")]
        [TestCase("The object will already get notified.", "The object already gets notified.")]
        [TestCase("The object will in turn get notified.", "The object in turn gets notified.")]
        [TestCase("The object will no longer get notified.", "The object no longer gets notified.")]
        [TestCase("The object will just be notified.", "The object just is notified.")]
        [TestCase("Will just be notified.", "Just is notified.")]
        [TestCase("The object will 'get' notified.", "The object 'gets' notified.")]
        [TestCase(@"The object will ""get"" notified.", @"The object ""gets"" notified.")]
        [TestCase("The objects will all be notified.", "The objects all are notified.")]
        [TestCase("The object will ALWAYS be notified.", "The object is always notified.")]
        [TestCase("The object will always be notified.", "The object is always notified.")]
        [TestCase("The object will always return.", "The object always returns.")]
        [TestCase("The objects will both be notified.", "The objects are both notified.")]
        [TestCase("The object will first get notified.", "The object first gets notified.")]
        [TestCase("The object will later get notified.", "The object later gets notified.")]
        [TestCase("The object will later on get notified.", "The object later on gets notified.")]
        [TestCase("The object will likely either get notified.", "The object likely either gets notified.")]
        [TestCase("The object will now get notified.", "The object now gets notified.")]
        [TestCase("The object will afterwards get notified.", "The object afterwards gets notified.")]
        [TestCase("The object will however only be the first.", "The object however only is the first.")]
        [TestCase("The object will either be the first or the second.", "The object either is the first or the second.")]
        [TestCase("The object will simply cause an issue.", "The object simply causes an issue.")]
        [TestCase("The manager will not create any issues.", "The manager does not create any issues.")]
        [TestCase("The managers will not create any issues.", "The managers do not create any issues.")]
        [TestCase("The managers will be successful.", "The managers are successful.")]
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

        [TestCase("This will start", "This starts", @"The return value will be <see langword=""false""/>.", @"The return value is <see langword=""false""/>.")]
        public void Code_gets_fixed_when_on_separate_lines(string originalPhrase, string fixedPhrase, string originalReturn, string fixedReturn)
        {
            const string Template = @"
using System;

public interface ITestMe
{
    /// <summary>
    /// #1# something.
    /// Even some other thing.
    /// </summary>
    /// <returns>#2#</returns>
    int DoSomething()
}
";

            VerifyCSharpFix(Template.Replace("#1#", originalPhrase).Replace("#2#", originalReturn), Template.Replace("#1#", fixedPhrase).Replace("#2#", fixedReturn));
        }

        protected override string GetDiagnosticId() => MiKo_2049_WillBePhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2049_WillBePhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2049_CodeFixProvider();
    }
}