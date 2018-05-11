using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3047_ValueConvertersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_converter_class() => No_issue_is_reported_for(@"
using System;

public namespace Bla.Blubb
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_converter_class_in_correct_namespace([ValueSource(nameof(ValidTypes))] string interfaceName) => No_issue_is_reported_for(@"
using System;

public namespace Bla.Blubb.Converters
{
    public class TestMe : " + interfaceName + @"
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_converter_class_in_wrong_namespace([ValueSource(nameof(ValidTypes))] string interfaceName) => An_issue_is_reported_for(@"
using System;

public namespace Bla.Blubb
{
    public class TestMe : " + interfaceName + @"
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3047_ValueConvertersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3047_ValueConvertersAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> ValidTypes() => new[]
                                                               {
                                                                   "IValueConverter",
                                                                   "IMultiValueConverter",
                                                                   "System.Windows.Data.IValueConverter",
                                                                   "System.Windows.Data.IMultiValueConverter",
                                                               };
    }
}