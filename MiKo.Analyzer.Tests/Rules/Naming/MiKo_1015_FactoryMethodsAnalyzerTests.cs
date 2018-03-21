using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1015_FactoryMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeReFactor
{
    public void DoSomething()
    {
    }
");

        [Test]
        public void No_issue_is_reported_for_empty_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
}
");

        [Test]
        public void No_issue_is_reported_for_constructor_of_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
    public TestMeFactory()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_factory_method() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
    public int CreateInt() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_factory_method() => An_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
    public int BuildInt() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_but_overridden_factory_method() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory : BaseFactory
{
    public override int BuildInt() => 42;
}
");

        protected override string GetDiagnosticId() => MiKo_1015_FactoryMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1015_FactoryMethodsAnalyzer();
    }
}