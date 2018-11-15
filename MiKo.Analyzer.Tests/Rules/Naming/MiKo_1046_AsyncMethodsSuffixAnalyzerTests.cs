using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1046_AsyncMethodsSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_async_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_async_void_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomethingAsync() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_async_void_method() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public async void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_Task_method() => No_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomethingAsync() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_Task_method() => An_issue_is_reported_for(@"
using System.Threading.Tasks;

public class TestMe
{
    public Task DoSomething() { }
}
");

        [TestCase(nameof(TestFixtureAttribute), nameof(TestAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseSourceAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TheoryAttribute))]
        [TestCase("TestFixture", "Test")]
        [TestCase("TestFixture", "TestCase")]
        [TestCase("TestFixture", "TestCaseSource")]
        [TestCase("TestFixture", "Theory")]
        [TestCase("TestClassAttribute", "TestMethodAttribute")]
        [TestCase("TestClass", "TestMethod")]
        [TestCase(nameof(TestFixtureAttribute), nameof(TearDownAttribute))]
        [TestCase("TestFixture", "TearDown")]
        [TestCase("TestClassAttribute", "TestCleanupAttribute")]
        [TestCase("TestClass", "TestCleanup")]
        [TestCase(nameof(TestFixtureAttribute), nameof(SetUpAttribute))]
        [TestCase("TestFixture", "SetUp")]
        [TestCase("TestClassAttribute", "TestInitializeAttribute")]
        [TestCase("TestClass", "TestInitialize")]
        public void No_issue_is_reported_for_test_method(string testClassAttribute, string testAttribute) => No_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_TaskFactory_method([ValueSource(nameof(TaskFactoryMethods))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName  + @"() { }
}
");


        protected override string GetDiagnosticId() => MiKo_1046_AsyncMethodsSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1046_AsyncMethodsSuffixAnalyzer();

        private static IEnumerable<string> TaskFactoryMethods() => typeof(TaskFactory).GetMethods().Concat(typeof(TaskFactory<int>).GetMethods()).Select(_ => _.Name).ToHashSet();
    }
}