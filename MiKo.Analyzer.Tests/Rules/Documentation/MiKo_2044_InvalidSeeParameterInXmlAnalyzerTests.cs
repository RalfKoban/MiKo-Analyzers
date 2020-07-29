using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2044_InvalidSeeParameterInXmlAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method_without_parameters() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething(int i) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([Values("see", "seealso")] string tag) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something with <" + tag + @" cref=""i"" />.
    /// </summary>
    public void DoSomething(int i) { }
}
");

        protected override string GetDiagnosticId() => MiKo_2044_InvalidSeeParameterInXmlAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2044_InvalidSeeParameterInXmlAnalyzer();
    }
}