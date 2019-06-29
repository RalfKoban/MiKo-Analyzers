using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_thrown_ArgumentException() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_thrown_ArgumentNullException() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentNullException();
    }
}
");

        [TestCase("\"x\", 42, \"some message\"")]
        [TestCase("nameof(x), 42, \"some message\"")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentOutOfRangeException(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", new Exception()")]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentOutOfRangeException(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentOutOfRangeException(" + parameters + @");
    }
}
");

        [TestCase("\"x\", 42, typeof(StringComparison)")]
        [TestCase("nameof(x), 42, typeof(StringComparison)")]
        public void No_issue_is_reported_for_correctly_thrown_InvalidEnumArgumentException(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", new Exception()")]
        public void An_issue_is_reported_for_incorrectly_thrown_InvalidEnumArgumentException(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new InvalidEnumArgumentException(" + parameters + @");
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer();
    }
}