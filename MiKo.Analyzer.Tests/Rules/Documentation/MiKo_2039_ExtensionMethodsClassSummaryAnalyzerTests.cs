using System.Collections.Generic;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2039_ExtensionMethodsClassSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_non_static_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int value) { }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_static_non_extension_class() => No_issue_is_reported_for(@"
public static class TestMe
{
    public static void DoSomething(int value) { }
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_extension_class() => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int value) { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_extension_class_([ValueSource(nameof(ValidPhrases))] string phrase) => No_issue_is_reported_for(@"
/// <summary>
/// " + phrase + @" something.
/// </summary>
public static class TestMeExtensions
{
    public static void DoSomething(this int value) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_extension_class() => An_issue_is_reported_for(@"
/// <summary>
/// Does something.
/// </summary>
public static class TestMeExtensions
{
    public static void DoSomething(this int value) { }
}
");

        [TestCase("Doing", @"Provides a set of <see langword=""static""/> methods for doing")]
        [TestCase("Contains extensions for", @"Provides a set of <see langword=""static""/> methods for")]
        [TestCase("Contains extension methods for", @"Provides a set of <see langword=""static""/> methods for")]
        [TestCase("Provides extensions for", @"Provides a set of <see langword=""static""/> methods for")]
        [TestCase("Provides extension methods for", @"Provides a set of <see langword=""static""/> methods for")]
        public void Code_gets_fixed_(string originalCode, string fixedCode)
        {
            const string Template = @"
/// <summary>
/// ### something.
/// </summary>
public static class TestMeExtensions
{
    public static void DoSomething(this int value) { }
}
";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        protected override string GetDiagnosticId() => MiKo_2039_ExtensionMethodsClassSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2039_ExtensionMethodsClassSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2039_CodeFixProvider();

        private static IEnumerable<string> ValidPhrases() => new[]
                                                                 {
                                                                     "Provides a set of <see langword=\"static\"/> methods for ",
                                                                     "Provides a set of <see langword=\"static\" /> methods for ",
                                                                 };
    }
}