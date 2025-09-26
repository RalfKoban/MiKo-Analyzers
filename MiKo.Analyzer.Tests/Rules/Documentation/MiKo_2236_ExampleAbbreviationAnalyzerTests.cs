using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2236_ExampleAbbreviationAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Phrases =
                                                   [
                                                       "It's e.g. something.",
                                                       "It is e.g. something.",
                                                       "It's i.e. something.",
                                                       "It is i.e. something.",
                                                       "(E.g. something)",
                                                       "(e.g. something)",
                                                       "It is eg. something.",
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

/// <summary>It is for example something.</summary>
/// <remarks>It is for example something.</remarks>
public class TestMe
{
    /// <summary>It is for example something.</summary>
    /// <remarks>It is for example something.</remarks>
    public event EventHandler<T> MyEvent;

    /// <summary>It is for example something.</summary>
    /// <remarks>It is for example something.</remarks>
    public void DoSomething() { }

    /// <summary>It is for example something.</summary>
    /// <remarks>It is for example something.</remarks>
    public int Age { get; set; }

    /// <summary>It is for example something.</summary>
    /// <remarks>It is for example something.</remarks>
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

        [TestCase("It's e.g. something", "It's for example something")]
        [TestCase("It's i.e. something", "It's for example something")]
        [TestCase("It's e. g. something", "It's for example something")]
        [TestCase("It's i. e. something", "It's for example something")]
        [TestCase("It's something (i.e. whatever)", "It's something (for example whatever)")]
        [TestCase("E.g. something", "For example something")]
        [TestCase("I.e. something", "For example something")]
        [TestCase("E. g. something", "For example something")]
        [TestCase("I. e. something", "For example something")]
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

        protected override string GetDiagnosticId() => MiKo_2236_ExampleAbbreviationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2236_ExampleAbbreviationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2236_CodeFixProvider();
    }
}