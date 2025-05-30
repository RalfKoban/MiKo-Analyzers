﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1082_PropertiesWithNumberSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_property_with_no_number_suffix() => No_issue_is_reported_for(@"

public class TestMe
{
    public int DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_with_number_suffix_if_type_of_property_has_no_number_suffix_([Range(0, 10)] int number) => No_issue_is_reported_for(@"

public class TestMe
{
    public object DoSomething" + number + @" { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_number_suffix_if_type_of_property_has_number_suffix_([Range(0, 10)] int number) => An_issue_is_reported_for(@"

public class TestMe
{
    public Int32 DoSomething" + number + @" { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_with_OS_bit_number_suffix_if_type_of_property_has_number_suffix_([Values(32, 64)] in int number) => No_issue_is_reported_for(@"

public class TestMe
{
    public Int32 DoSomething" + number + @" { get; set; }
}
");

        [Test]
        public void Code_gets_fixed() => VerifyCSharpFix(
                                                     "class TestMe { int I1 { get; set; } }",
                                                     "class TestMe { int I { get; set; } }");

        protected override string GetDiagnosticId() => MiKo_1082_PropertiesWithNumberSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1082_PropertiesWithNumberSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1082_CodeFixProvider();
    }
}