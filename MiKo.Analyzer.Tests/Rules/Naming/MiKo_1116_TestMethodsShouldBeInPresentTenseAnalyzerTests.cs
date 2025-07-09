using System;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1116_TestMethodsShouldBeInPresentTenseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] NonPresentPhrases = ["Was", "Returned", "Will", "Threw"];

        [Test]
        public void No_issue_is_reported_for_non_test_method_with_present_tense() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_method_with_([ValueSource(nameof(NonPresentPhrases))] string phrase) => No_issue_is_reported_for(@"
public class TestMe
{
    public int " + phrase + @"Something() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_present_tense() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public int DoSomething() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_([ValueSource(nameof(NonPresentPhrases))] string phrase) => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public int " + phrase + @"Something() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_lower_case_([ValueSource(nameof(NonPresentPhrases))] string phrase) => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public int Method_" + phrase.ToLowerCaseAt(0) + @"_something() => 42;
}
");

        protected override string GetDiagnosticId() => MiKo_1116_TestMethodsShouldBeInPresentTenseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1116_TestMethodsShouldBeInPresentTenseAnalyzer();
    }
}