using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1060_UseNotFoundInsteadOfMissingAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_normal_type() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeMissing
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_exception() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMeNotFoundException : Exception
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_enum_member() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public enum TestMe
    {
        None = 0,
        NotFound = 1,
        OK = 2,
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_exception([Values("TestMeMissingException", "GetTestMeFailedException")] string name) => An_issue_is_reported_for(@"
using System;

public class " + name + @" : Exception
{
    public void DoSomething(object[] values)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_enum_member([Values("Missing", "GetFailed")] string name) => An_issue_is_reported_for(@"
using System;

public enum TestMe
{
    None = 0,
    " + name + @" = 1,
}
");

        protected override string GetDiagnosticId() => MiKo_1060_UseNotFoundInsteadOfMissingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1060_UseNotFoundInsteadOfMissingAnalyzer();
    }
}