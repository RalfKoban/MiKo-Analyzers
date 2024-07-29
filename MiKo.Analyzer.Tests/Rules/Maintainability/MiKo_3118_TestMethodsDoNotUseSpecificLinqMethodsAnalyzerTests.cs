using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3118_TestMethodsDoNotUseSpecificLinqMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_method_using_Linq() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public bool DoSomething(IEnumerable<int> values)
    {
         return values.Skip(1).Take(3).Any(_ => _ == 2);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_method() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_using_unproblematic_Linq() => No_issue_is_reported_for(@"
using System.Collections.Generic;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        var result = Enumerable.Empty<int>();

        Assert.That(result, Is.Empty);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_using_Linq_in_Moq_matcher() => No_issue_is_reported_for(@"
using System.Collections.Generic;

using NUnit.Framework;

using Moq;

public interface ITestee
{
    void DoSomething(IEnumerable<string> texts);
}

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        var testee = new Mock<ITestee>();

        testee.Verify(_ => _.DoSomething(It.Is<IEnumerable<string>>(__ => __.Any())
    }
}
");

        [TestCase(nameof(Enumerable.Skip) + "(1)")]
        [TestCase(nameof(Enumerable.SkipLast) + "()")]
        [TestCase(nameof(Enumerable.SkipWhile) + "(true)")]
        [TestCase(nameof(Enumerable.Take) + "(1)")]
        [TestCase(nameof(Enumerable.TakeLast) + "()")]
        [TestCase(nameof(Enumerable.TakeWhile) + "(true)")]
        [TestCase(nameof(Enumerable.Any) + "(_ => _ == 2)")]
        [TestCase(nameof(Enumerable.All) + "(_ => _ == 2)")]
        [TestCase(nameof(Enumerable.FirstOrDefault) + "()")]
        [TestCase(nameof(Enumerable.LastOrDefault) + "()")]
        [TestCase(nameof(Enumerable.SingleOrDefault) + "()")]
        [TestCase(nameof(Enumerable.Single) + "()")]
        public void An_issue_is_reported_for_test_method_using_problematic_Linq_(string call) => An_issue_is_reported_for(@"
using System.Collections.Generic;

using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        int[] values = { 1, 2, 3 };

        var result = values." + call + @";

        Assert.That(result, Is.Not.Null);
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3118_TestMethodsDoNotUseSpecificLinqMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3118_TestMethodsDoNotUseSpecificLinqMethodsAnalyzer();
    }
}