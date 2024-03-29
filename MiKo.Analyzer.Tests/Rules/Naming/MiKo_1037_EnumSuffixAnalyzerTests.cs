﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1037_EnumSuffixAnalyzerTests : CodeFixVerifier
    {
        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("struct")]
        public void No_issue_is_reported_for_type_with_correct_name_(string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe
{
}
");

        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("struct")]
        public void No_issue_is_reported_for_type_with_Enum_name_only_(string type) => No_issue_is_reported_for(@"
public " + type + @" Enum
{
}
");

        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("struct")]
        public void An_issue_is_reported_for_type_with_Enum_suffix_(string type) => An_issue_is_reported_for(@"
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

        [TestCase("interface")]
        [TestCase("class")]
        [TestCase("struct")]
        [TestCase("enum")]
        public void Code_gets_fixed_(string type) => VerifyCSharpFix(
                                                                 "using System; " + type + " TestMeEnum { }",
                                                                 "using System; " + type + " TestMe { }");

        [Test]
        public void Code_gets_fixed_for_Flags() => VerifyCSharpFix(
                                                               "using System; [Flags] enum TestMeEnum { }",
                                                               "using System; [Flags] enum TestMes { }");

        protected override string GetDiagnosticId() => MiKo_1037_EnumSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1037_EnumSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1037_CodeFixProvider();
    }
}