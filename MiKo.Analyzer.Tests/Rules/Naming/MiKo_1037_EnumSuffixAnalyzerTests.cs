using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1037_EnumSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("struct")]
        public void No_issue_is_reported_for_type_with_correct_name(string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe
{
}
");

        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("struct")]
        public void No_issue_is_reported_for_type_with_Enum_name_only(string type) => No_issue_is_reported_for(@"
public " + type + @" Enum
{
}
");

        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("struct")]
        public void An_issue_is_reported_for_type_with_Enum_suffix(string type) => An_issue_is_reported_for(@"
public " + type + @" TestMeEnum
{
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_Enum_suffix() => An_issue_is_reported_for(@"
public enum TestMeEnum
{
}
");

        [Test]
        public void An_issue_is_reported_for_flags_enum_with_Enum_suffix() => An_issue_is_reported_for(@"
using System;

[Flags]
public enum TestMeEnums
{
}
");

        protected override string GetDiagnosticId() => MiKo_1037_EnumSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1037_EnumSuffixAnalyzer();
    }
}