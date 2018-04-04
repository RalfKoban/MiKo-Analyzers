using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3011_ArgumentExceptionsParamNameAnalyzerTests : CodeFixVerifier
    {
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", \"x\", null")]
        [TestCase("\"some message\", nameof(x), null")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentException(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"some message\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\", null")]
        [TestCase("nameof(x), \"some message\", null")]
        [TestCase("\"some message\", \"X\"")]
        [TestCase("\"some message\", nameof(TestMe)")]
        [TestCase("\"some message\", \"X\", null")]
        [TestCase("\"some message\", nameof(TestMe), null")]
        [TestCase("\"some message\", new Exception()")]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentException(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentException(" + parameters + @");
    }
}
");

        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        public void No_issue_is_reported_for_correctly_thrown_ArgumentNullException(string parameters) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentNullException(" + parameters + @");
    }
}
");

        [TestCase("")]
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"X\", \"some message\"")]
        [TestCase("nameof(TestMe), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", new Exception()")]
        public void An_issue_is_reported_for_incorrectly_thrown_ArgumentNullException(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x)
    {
        if (x == 42) throw new ArgumentNullException(" + parameters + @");
    }
}
");

        [TestCase("\"x\"")]
        [TestCase("nameof(x)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
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
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"X\", \"some message\"")]
        [TestCase("nameof(TestMe), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", new Exception()")]
        [TestCase("\"some message\", 42, \"x\"")]
        [TestCase("\"some message\", 42, nameof(x)")]
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

        [TestCase("\"x\", 42, \"some message\"")]
        [TestCase("nameof(x), 42, \"some message\"")]
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
        [TestCase("\"X\"")]
        [TestCase("nameof(TestMe)")]
        [TestCase("\"x\", \"some message\"")]
        [TestCase("nameof(x), \"some message\"")]
        [TestCase("\"X\", \"some message\"")]
        [TestCase("nameof(TestMe), \"some message\"")]
        [TestCase("\"some message\"")]
        [TestCase("\"some message\", \"x\"")]
        [TestCase("\"some message\", nameof(x)")]
        [TestCase("\"some message\", new Exception()")]
        [TestCase("\"some message\", 42, typeof(StringComparison)")]
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

        protected override string GetDiagnosticId() => MiKo_3011_ArgumentExceptionsParamNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3011_ArgumentExceptionsParamNameAnalyzer();
    }
}