using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1078_BuilderMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_builder_class() => No_issue_is_reported_for(@"
using System;

public class TestMeReFactor
{
    public void DoSomething()
    {
    }
");

        [Test]
        public void No_issue_is_reported_for_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
    public object Create() => new object();

    public int CreateInt() => 42;
");

        [Test]
        public void No_issue_is_reported_for_empty_builder_class() => No_issue_is_reported_for(@"
using System;

public class TestMeBuilder
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_builder_method() => No_issue_is_reported_for(@"
using System;

public class TestMeBuilder
{
    public int BuildInt() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_builder_property() => No_issue_is_reported_for(@"
using System;

public class TestMeBuilder
{
    public int CreateId { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_private_builder_method() => No_issue_is_reported_for(@"
using System;

public class TestMeBuilder
{
    private int CreateInt() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_constructor() => No_issue_is_reported_for(@"
using System;

public class CreateTestMeBuilder
{
    public CreateTestMeBuilder() { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_builder_method() => An_issue_is_reported_for(@"
using System;

public class TestMeBuilder
{
    public int CreateInt() => 42;
}
");

        [TestCase("CreateTestMe", "BuildTestMe")]
        [TestCase("Create", "Build")]
        public void Code_gets_fixed_for_(string originalName, string fixedName)
        {
            const string Template = @"
using System;

public class TestMe
{
}

public class TestMeBuilder
{
    public TestMe ###() => new TestMe();
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1078_BuilderMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1078_BuilderMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1078_CodeFixProvider();
    }
}