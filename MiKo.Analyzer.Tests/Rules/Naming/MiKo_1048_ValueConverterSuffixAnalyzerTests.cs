using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1048_ValueConverterSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ConverterInterfaces =
            {
                "IValueConverter",
                "IMultiValueConverter",
                "System.Windows.Data.IValueConverter",
                "System.Windows.Data.IMultiValueConverter",
            };

        [Test]
        public void No_issue_is_reported_for_non_converter_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_converter_class_([ValueSource(nameof(ConverterInterfaces))] string interfaceName) => No_issue_is_reported_for(@"
using System;
using System.Windows.Data;

public class TestMeConverter : " + interfaceName + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_converter_class_([ValueSource(nameof(ConverterInterfaces))] string interfaceName) => An_issue_is_reported_for(@"
using System;
using System.Windows.Data;

public class TestMe : " + interfaceName + @"
{
}
");

        [Test]
        public void Code_gets_fixed_() => VerifyCSharpFix(
                                                      @"using System; class TestMe : System.Windows.Data.IValueConverter { }",
                                                      @"using System; class TestMeConverter : System.Windows.Data.IValueConverter { }");

        protected override string GetDiagnosticId() => MiKo_1048_ValueConverterSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1048_ValueConverterSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1048_CodeFixProvider();
    }
}