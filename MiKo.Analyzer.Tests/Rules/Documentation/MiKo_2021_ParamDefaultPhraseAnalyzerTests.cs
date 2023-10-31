using System;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2021_ParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o) { }
}
");

        [TestCase("bool")]
        [TestCase("System.Boolean")]
        [TestCase("System.StringComparison")]
        [TestCase(nameof(Boolean))]
        [TestCase(nameof(StringComparison))]
        public void No_issue_is_reported_for_method_with_(string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(" + type + @" o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_out_parameter() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(out object o) { }
}
");

        [TestCase("A whatever.")]
        [TestCase("An whatever.")]
        [TestCase("The whatever.")]
        [TestCase("a whatever")]
        [TestCase("an whatever")]
        [TestCase("the whatever")]
        [TestCase("unused")]
        [TestCase("Unused")]
        [TestCase("Unused.")]
        public void No_issue_is_reported_for_method_with_correct_comment_(string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correct_comment_and_line_break_after_first_word_([Values("A", "An", "The")] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>
    /// " + comment + @"
    /// <see cref=""object"">
    /// something
    /// </see>
    /// to do.
    /// </param>
    public void DoSomething(object o) { }
}
");

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(object o) { }
}
");

        [TestCase("<summary />")]
        [TestCase("<inheritdoc />")]
        [TestCase("<exclude />")]
        public void No_issue_is_reported_for_method_with_missing_documentation_(string xmlElement) => No_issue_is_reported_for(@"
public class TestMe
{
    /// " + xmlElement + @"
    public void DoSomething(object o) { }
}
");

        protected override string GetDiagnosticId() => MiKo_2021_ParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2021_ParamDefaultPhraseAnalyzer();
    }
}