using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1039_ExtensionMethodsParameterAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectParameterNames = { "value", "source", "values" };
        private static readonly string[] WrongParameterNames = { "o", "something", "v" };
        private static readonly string[] CorrectConversionParameterNames = { "source" };
        private static readonly string[] WrongConversionParameterNames = { "o", "something", "v", "value", "values" };

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
        public void No_issue_is_reported_for_extension_method_with_correct_parameter_name([ValueSource(nameof(CorrectParameterNames))] string name) => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int " + name + @") { }
}
");

        [Test]
        public void An_issue_is_reported_for_extension_method_with_incorrect_parameter_name([ValueSource(nameof(WrongParameterNames))] string name) => An_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int " + name + @") { }
}
");

        [Test]
        public void No_issue_is_reported_for_ToXyz_extension_method_with_correct_parameter_name([ValueSource(nameof(CorrectConversionParameterNames))] string name) => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static int ToSomething(this int " + name + @") => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_ToXyz_extension_method_with_incorrect_parameter_name([ValueSource(nameof(WrongConversionParameterNames))] string name) => An_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static int ToSomething(this int " + name + @") => 42;
}
");

        protected override string GetDiagnosticId() => MiKo_1039_ExtensionMethodsParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1039_ExtensionMethodsParameterAnalyzer();
    }
}