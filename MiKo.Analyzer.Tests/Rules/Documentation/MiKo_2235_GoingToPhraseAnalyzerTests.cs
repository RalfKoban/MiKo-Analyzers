using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2235_GoingToPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] XmlTags = ["summary", "remarks", "returns", "example", "value", "exception"];

        private static readonly string[] Phrases =
                                                   [
                                                       "It's going to be something.",
                                                       "It is going to be something.",
                                                       "It is (going to be) something.",
                                                       "We're going to be something.",
                                                       "We are going to be something.",
                                                       "We are (going to be) something.",
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

/// <summary>Will be something.</summary>
/// <remarks>Will be something.</remarks>
public class TestMe
{
    /// <summary>Will be something.</summary>
    /// <remarks>Will be something.</remarks>
    public event EventHandler<T> MyEvent;

    /// <summary>Will be something.</summary>
    /// <remarks>Will be something.</remarks>
    public void DoSomething() { }

    /// <summary>Will be something.</summary>
    /// <remarks>Will be something.</remarks>
    public int Age { get; set; }

    /// <summary>Will be something.</summary>
    /// <remarks>Will be something.</remarks>
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

        [TestCase("It's going to be something", "It will be something")]
        [TestCase("That's going to be something", "That will be something")]
        [TestCase("where it is going to be something", "where it will be something")]
        [TestCase("is going to be something", "will be something")]
        [TestCase("(We're going to be something)", "(We will be something)")]
        [TestCase("You're going to be something", "You will be something")]
        [TestCase("are going to be something", "will be something")]
        [TestCase("are (going to be) something", "are (will be) something")]
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

        protected override string GetDiagnosticId() => MiKo_2235_GoingToPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2235_GoingToPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2235_CodeFixProvider();
    }
}