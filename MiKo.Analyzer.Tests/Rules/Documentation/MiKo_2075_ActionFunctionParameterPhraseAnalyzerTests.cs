using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public class MiKo_2075_ActionFunctionParameterPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ProblematicWords = { "action", "func", "function", "Action", "Func", "Function" };

        [Test]
        public void No_issue_is_reported_for_uncommented_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void DoSomething()
    { }

    public string Property
    {
        get => m_field;
        set => m_field = value;
    }

    private string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_commented_class() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
    /// <summary>
    /// Some summary.
    /// </summary>
    public event EventHandler MyEvent;

    /// <summary>
    /// Some summary.
    /// </summary>
    public void DoSomething()
    { }

    /// <summary>
    /// Some summary.
    /// </summary>
    public string Property
    {
        get => m_field;
        set => m_field = value;
    }

    /// <summary>
    /// Some summary.
    /// </summary>
    private string m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_class_comment_that_contains_([ValueSource(nameof(ProblematicWords))] string word) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Some " + word + @".
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_event_comment_that_contains_([ValueSource(nameof(ProblematicWords))] string word) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some " + word + @".
    /// </summary>
    public event EventHandler MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_method_comment_that_contains_([ValueSource(nameof(ProblematicWords))] string word) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some " + word + @".
    /// </summary>
    public void DoSomething()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_property_comment_that_contains_([ValueSource(nameof(ProblematicWords))] string word) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some " + word + @".
    /// </summary>
    public string Property
    {
        get => m_field;
        set => m_field = value;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_comment_that_contains_([ValueSource(nameof(ProblematicWords))] string word) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Some " + word + @".
    /// </summary>
    private string m_field;
}
");

        [Test]
        public void Code_gets_fixed_for_method_comment_that_contains_([ValueSource(nameof(ProblematicWords))] string word)
        {
            const string Template = @"
using System;

public class TestMe
{
    /// <summary>
    /// Some ###.
    /// </summary>
    public void DoSomething()
    { }
}
";

            VerifyCSharpFix(Template.Replace("###", word), Template.Replace("###", "callback"));
        }

        protected override string GetDiagnosticId() => MiKo_2075_ActionFunctionParameterPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2075_ActionFunctionParameterPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2075_CodeFixProvider();
    }
}