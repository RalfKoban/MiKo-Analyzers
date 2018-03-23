using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1038_ExtensionMethodsSuffixAnalyzerTests : CodeFixVerifier
    {
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
        public void An_issue_is_reported_for_extension_class_with_incorrect_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
public static class " + name + @"
{
    public static void DoSomething(this int value) { }
}
");

        protected override string GetDiagnosticId() => MiKo_1038_ExtensionMethodsSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1038_ExtensionMethodsSuffixAnalyzer();

        private static IEnumerable<string> WrongNames() => new[]
                                                               {
                                                                   "ExtensionsClass",
                                                                   "SomeExtensionMethods",
                                                                   "Something",
                                                               };
    }
}