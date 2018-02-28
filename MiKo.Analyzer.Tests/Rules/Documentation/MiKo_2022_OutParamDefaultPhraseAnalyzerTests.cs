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

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        [TestCase("On true, returns something.")]
        [TestCase("On <see langword='true' />, returns something.")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(out object o) { }
}
");

        protected override string GetDiagnosticId() => MiKo_2022_OutParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2022_OutParamDefaultPhraseAnalyzer();
    }
}