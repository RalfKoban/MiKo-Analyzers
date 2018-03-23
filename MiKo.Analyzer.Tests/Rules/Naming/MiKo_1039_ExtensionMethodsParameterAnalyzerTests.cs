using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1039_ExtensionMethodsParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_normal_method() => No_issue_is_reported_for(@"
public static class TestMe
{
    public static void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_extension_method_with_correct_parameter_name([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int " + name + @") { }
}
");

        [Test]
        public void An_issue_is_reported_for_extension_method_with_incorrect_parameter_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int " + name + @") { }
}
");

        protected override string GetDiagnosticId() => MiKo_1039_ExtensionMethodsParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1039_ExtensionMethodsParameterAnalyzer();

        private static IEnumerable<string> CorrectNames() => new[] { "value", "source" };

        private static IEnumerable<string> WrongNames() => new[] { "o", "something", "v" };
    }
}