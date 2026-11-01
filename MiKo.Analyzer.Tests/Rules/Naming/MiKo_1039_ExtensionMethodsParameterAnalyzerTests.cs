using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1039_ExtensionMethodsParameterAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectParameterNames = ["value", "source", "values"];
        private static readonly string[] WrongParameterNames = ["o", "something", "v"];
        private static readonly string[] CorrectConversionParameterNames = ["source"];
        private static readonly string[] WrongConversionParameterNames = ["o", "something", "v", "value", "values"];
        private static readonly string[] ConversionMethodPrefixes = ["To", "From"];

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_plain_method() => No_issue_is_reported_for(@"
public static class TestMe
{
    public static void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_extension_method_with_parameter_named_([ValueSource(nameof(CorrectParameterNames))] string name) => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int " + name + @") { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_extension_method_conversion_with_parameter_named_(
                                                                                           [ValueSource(nameof(ConversionMethodPrefixes))] string prefix,
                                                                                           [Values("Something", "")] string methodName,
                                                                                           [ValueSource(nameof(CorrectConversionParameterNames))] string name)
            => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static int " + prefix + methodName + "(this int " + name + @") => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_specific_string_extension_method() => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static string FormatWith(this string format, object arg0) => string.Format(format, arg0);
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void No_issue_is_reported_for_parameterless_method_in_extension_block() => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    extension(int value)
    {
        public void DoSomething() { }
    }
}
", languageVersion: LanguageVersion.CSharp14);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void No_issue_is_reported_for_first_argument_of_method_in_extension_block() => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    extension(int value)
    {
        public void DoSomething(int o) { }
    }
}
", languageVersion: LanguageVersion.CSharp14);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void No_issue_is_reported_for_conversion_and_formatting_methods_in_extension_block() => No_issue_is_reported_for(@"
public static class TestMeExtensions
{
    extension(int value)
    {
        public int FromSomething() => 42;

        public int ToSomething() => 42;

        public string FormatWith(object arg0) => string.Format(value, arg0);
    }
}
", languageVersion: LanguageVersion.CSharp14);

        [Test]
        public void An_issue_is_reported_for_extension_method_with_parameter_named_([ValueSource(nameof(WrongParameterNames))] string name) => An_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static void DoSomething(this int " + name + @") { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_extension_method_with_parameter_named_(
                                                                                [ValueSource(nameof(ConversionMethodPrefixes))] string prefix,
                                                                                [Values("Something", "2D", "")] string methodName,
                                                                                [ValueSource(nameof(WrongConversionParameterNames))] string name)
            => An_issue_is_reported_for(@"
public static class TestMeExtensions
{
    public static int " + prefix + methodName + "Something(this int " + name + @") => 42;
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

            VerifyCSharpFix(Template.Replace("#PREFIX#", prefix).Replace("#NAME#", "i"), Template.Replace("#PREFIX#", prefix).Replace("#NAME#", "source"));
        }

        [Test]
        public void Code_gets_fixed_for_plain_parameter()
        {
            const string Template = @"
public static class TestMeExtensions
{
    public static int Something(this int #NAME#) => 42;
}
";

            VerifyCSharpFix(Template.Replace("#NAME#", "i"), Template.Replace("#NAME#", "value"));
        }

        [Test]
        public void Code_gets_fixed_for_collection_parameter()
        {
            const string Template = @"
using System.Collections.Generic;

public static class TestMeExtensions
{
    public static int Something(this IEnumerable<int> #NAME#) => 42;
}
";

            VerifyCSharpFix(Template.Replace("#NAME#", "i"), Template.Replace("#NAME#", "values"));
        }

        [Test]
        public void Code_gets_fixed_for_ValueType_parameter_that_is_a_collection()
        {
            const string Template = @"
using System.Collections.Generic;

using using Microsoft.CodeAnalysis;

public static class TestMeExtensions
{
    public static int Something(this SyntaxList<XmlNodeSyntax> #NAME#) => 42;
}
";

            VerifyCSharpFix(Template.Replace("#NAME#", "nodes"), Template.Replace("#NAME#", "values"));
        }

        [Test]
        public void Code_gets_fixed_for_specific_string_extension_method()
        {
            const string Template = @"
using System.Collections.Generic;

public static class TestMeExtensions
{
    public static string FormatWith(this string #NAME#, object arg0) => string.Format(#NAME#, arg0);
}
";

            VerifyCSharpFix(Template.Replace("#NAME#", "i"), Template.Replace("#NAME#", "format"));
        }

        [Test]
        public void Code_gets_fixed_for_conversion_method_in_extension_block_([ValueSource(nameof(ConversionMethodPrefixes))] string prefix)
        {
            const string Template = @"
public static class TestMeExtensions
{
    extension(int #NAME#)
    {
        public int #PREFIX#Something() => 42;
    }
}
";

            VerifyCSharpFix(Template.Replace("#PREFIX#", prefix).Replace("#NAME#", "i"), Template.Replace("#PREFIX#", prefix).Replace("#NAME#", "value"), languageVersion: LanguageVersion.CSharp14);
        }

        [Test]
        public void Code_gets_fixed_for_string_format_method_in_extension_block()
        {
            const string Template = @"
public static class TestMeExtensions
{
    extension(string #NAME#)
    {
        public string FormatWith(object arg0) => string.Format(#NAME#, arg0);
    }
}
";

            VerifyCSharpFix(Template.Replace("#NAME#", "i"), Template.Replace("#NAME#", "value"), languageVersion: LanguageVersion.CSharp14);
        }

        [Test]
        public void Code_gets_fixed_for_conversion_and_string_format_methods_in_extension_block()
        {
            const string Template = @"
public static class TestMeExtensions
{
    extension(int #NAME#)
    {
        public int FromSomething() => 42;

        public int ToSomething() => 42;

        public string FormatWith(object arg0) => string.Format(#NAME#, arg0);
    }
}
";

            VerifyCSharpFix(Template.Replace("#NAME#", "i"), Template.Replace("#NAME#", "value"), languageVersion: LanguageVersion.CSharp14);
        }

        [Test]
        public void CodeFixProvider_does_not_crash_with_InvalidOperationException_due_to_incomplete_code()
        {
            const string UncompilableCode = @"
using System.Collections.Generic;

public static class TestMeExtensions
{
    public static void Bla(this Docu)
    {
        .DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens)
    }
}
";

            VerifyCSharpFix(UncompilableCode, UncompilableCode);
        }

        protected override string GetDiagnosticId() => MiKo_1039_ExtensionMethodsParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1039_ExtensionMethodsParameterAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1039_CodeFixProvider();
    }
}