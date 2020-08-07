using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2022_OutParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(out object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_out_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correct_comment_phrase() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>On successful return, contains the object.</param>
    public void DoSomething(out object o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_correct_comment_phrase_for_bool() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>On successful return, indicates the object.</param>
    public void DoSomething(out bool o) { }
}
");

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        [TestCase("On true, returns something.")]
        [TestCase("On <see langword='true' />, returns something.")]
        [TestCase("On successful return, indicates something.")]
        [TestCase("")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(out object o) { }
}
");

        [TestCase("On true, returns something.")]
        [TestCase("On false, returns something.")]
        [TestCase("On successful return, contains something.")]
        [TestCase("")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase_on_bool_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='b'>" + comment + @"</param>
    public void DoSomething(out bool b) { }
}
");

        [TestCase("<summary />")]
        [TestCase("<inheritdoc />")]
        [TestCase("<exclude />")]
        public void No_issue_is_reported_for_method_with_missing_documentation_(string xmlElement) => No_issue_is_reported_for(@"
public class TestMe
{
    /// " + xmlElement + @"
    public void DoSomething(out object o) { }
}
");

        protected override string GetDiagnosticId() => MiKo_2022_OutParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2022_OutParamDefaultPhraseAnalyzer();
    }
}