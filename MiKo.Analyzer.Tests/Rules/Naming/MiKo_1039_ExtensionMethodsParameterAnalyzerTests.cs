using Microsoft.CodeAnalysis.CodeFixes;
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
        private static readonly string[] ConversionMethodPrefixes = { "To", "From" };

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
        public void No_issue_is_reported_for_extension_method_with_correct_parameter_name_([ValueSource(nameof(CorrectParameterNames))] string name) => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int " + name + @") { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_extension_method_with_correct_parameter_name_(
            [ValueSource(nameof(ConversionMethodPrefixes))] string prefix,
            [Values("Something", "")] string methodName,
            [ValueSource(nameof(CorrectConversionParameterNames))] string name)
            => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static int " + prefix + methodName + @"(this int " + name + @") => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_extension_method_with_incorrect_parameter_name_([ValueSource(nameof(WrongParameterNames))] string name) => An_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int " + name + @") { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_extension_method_with_incorrect_parameter_name_(
                                                                                    [ValueSource(nameof(ConversionMethodPrefixes))] string prefix,
                                                                                    [Values("Something", "")] string methodName,
                                                                                    [ValueSource(nameof(WrongConversionParameterNames))] string name)
            => An_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static int " + prefix + methodName + @"Something(this int " + name + @") => 42;
}
");

        [Test]
        public void Code_gets_fixed_for_extension_method_([ValueSource(nameof(ConversionMethodPrefixes))] string prefix)
        {
            const string Template = @"
public static class TestMeExtensions
{
    public static int #PREFIX#Something(this int #NAME#) => 42;
}
";

            var originalCode = Template.Replace("#PREFIX#", prefix).Replace("#NAME#", "i");
            var fixedCode = Template.Replace("#PREFIX#", prefix).Replace("#NAME#", "source");

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_normal_parameter()
        {
            const string Template = @"
public static class TestMeExtensions
{
    public static int Something(this int #NAME#) => 42;
}
";

            var originalCode = Template.Replace("#NAME#", "i");
            var fixedCode = Template.Replace("#NAME#", "value");

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_collection_parameter_()
        {
            const string Template = @"
using System.Collections.Generic;

public static class TestMeExtensions
{
    public static int Something(this IEnumerable<int> #NAME#) => 42;
}
";

            var originalCode = Template.Replace("#NAME#", "i");
            var fixedCode = Template.Replace("#NAME#", "values");

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1039_ExtensionMethodsParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1039_ExtensionMethodsParameterAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1039_CodeFixProvider();
    }
}