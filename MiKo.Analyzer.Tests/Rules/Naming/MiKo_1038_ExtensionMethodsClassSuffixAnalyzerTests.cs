using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1038_ExtensionMethodsClassSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongNames =
                                                      {
                                                          "Extension",
                                                          "ExtensionClass",
                                                          "ExtensionsClass",
                                                          "SomeExtensionMethods",
                                                          "SomeExtensionMethod",
                                                          "Something",
                                                      };

        [Test]
        public void No_issue_is_reported_for_struct() => No_issue_is_reported_for(@"
public struct TestMe
{
    public int DoSomething;
}
");

        [Test]
        public void No_issue_is_reported_for_non_extension_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_static_non_extension_class() => No_issue_is_reported_for(@"
public static class TestMe
{
    public static void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_extension_class_with_correct_name() => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int value) { }
}
");

        [Test]
        public void An_issue_is_reported_for_extension_class_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
public static class " + name + @"
{
    public static void DoSomething(this int value) { }
}
");

        [TestCase("ExtensionClass")]
        [TestCase("ExtensionsClass")]
        [TestCase("ExtensionMethod")]
        [TestCase("ExtensionMethods")]
        [TestCase("Extension")]
        public void Code_gets_fixed_(string name) => VerifyCSharpFix(
                                                                 "public static class TestMe" + name + " { public static void DoSomething(this int value) { } }",
                                                                 "public static class TestMeExtensions { public static void DoSomething(this int value) { } }");

        protected override string GetDiagnosticId() => MiKo_1038_ExtensionMethodsClassSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1038_ExtensionMethodsClassSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1038_CodeFixProvider();
    }
}