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

        [TestCase("Contains extension methods for", "")]
        [TestCase("Contains extension methods to", "")]
        [TestCase("Contains extensions for", "")]
        [TestCase("Contains extensions to", "")]
        [TestCase("Provides extension methods for", "")]
        [TestCase("Provides extension methods to", "")]
        [TestCase("Provides extension mehtods for", "")] // typo by intent
        [TestCase("Provides extension mehtods to", "")] // typo by intent
        [TestCase("Provides extensions for", "")]
        [TestCase("Provides extensions to", "")]
        [TestCase(@"Extension methods for", @"")]
        [TestCase(@"Extension methods to", @"")]
        [TestCase(@"Extension methods used in", @"")]
        [TestCase(@"Extensions for", @"")]
        [TestCase(@"Extensions to", @"")]
        [TestCase(@"Extensions used in", @"")]
        [TestCase(@"Offers extension methods for", "")]
        [TestCase(@"Offers extension methods to", "")]
        [TestCase(@"Offers extensions for", "")]
        [TestCase(@"Offers extensions to", "")]
        [TestCase(@"Offers the extension for", "")]
        [TestCase(@"Offers the extension to", "")]
        [TestCase(@"Offers the extension method for", "")]
        [TestCase(@"Offers the extension method to", "")]
        [TestCase(@"Static collection of extension methods for", "")]
        [TestCase(@"Static collection of extension methods to", "")]
        [TestCase(@"Static collection of extensions for", "")]
        [TestCase(@"Static collection of extensions to", "")]
        [TestCase(@"The extension methods for", "")]
        [TestCase(@"The extension methods to", "")]
        [TestCase(@"The extensions for", "")]
        [TestCase(@"The extensions to", "")]
        [TestCase("Doing", @" doing")]
        [TestCase(@"Extension methods for <see cref=""String""/>.", @" <see cref=""String""/>.")]
        [TestCase(@"Extensions for <see cref=""String""/>.", @" <see cref=""String""/>.")]
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

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", @"Provides a set of <see langword=""static""/> methods for" + fixedCode));
        }

        protected override string GetDiagnosticId() => MiKo_2039_ExtensionMethodsClassSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2039_ExtensionMethodsClassSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2039_CodeFixProvider();

        private static IEnumerable<string> ValidPhrases() => new[]
                                                                 {
                                                                     @"Provides a set of <see langword=""static""/> methods for ",
                                                                     @"Provides a set of <see langword=""static"" /> methods for ",
                                                                 };
    }
}