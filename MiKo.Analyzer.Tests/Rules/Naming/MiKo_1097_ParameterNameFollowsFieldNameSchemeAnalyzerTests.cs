using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1097_ParameterNameFollowsFieldNameSchemeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object value) { }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_with_wrong_prefix_([Values("m_", "s_", "t_", "_", "m", "s", "t")] string prefix) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object " + prefix + @"Value) { }
}
");

        [Test]
        public void Code_gets_fixed_for_parameter_with_wrong_prefix_([Values("m_", "s_", "t_", "_", "m", "s", "t")] string prefix)
        {
            const string Template = @"
using System;

public class TestMe
{
    public void DoSomething(object #1#) { }
}
";

            VerifyCSharpFix(Template.Replace("#1#", prefix + "Value"), Template.Replace("#1#", "value"));
        }

        protected override string GetDiagnosticId() => MiKo_1097_ParameterNameFollowsFieldNameSchemeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1097_ParameterNameFollowsFieldNameSchemeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1097_CodeFixProvider();
    }
}