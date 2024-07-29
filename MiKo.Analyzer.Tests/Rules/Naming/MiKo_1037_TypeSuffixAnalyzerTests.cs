using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1037_TypeSuffixAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] NonEnumTypes = ["interface", "class", "struct", "record"];

        private static readonly string[] WrongTypeNames = ["Type", "Interface", "Class", "Struct", "Record", "Enum"];

        private static readonly string[] WrongEnumNames = ["Type", "Struct", "Record", "Enum"];

        private static readonly string[] AllowedEnumNames = ["Interface", "Class"];

        [Test]
        public void No_issue_is_reported_for_correct_name_on_enum() => No_issue_is_reported_for(@"
public enum TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correct_name_on_([ValueSource(nameof(NonEnumTypes))] string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_value_as_name_only_on_enum_([ValueSource(nameof(WrongEnumNames))] string typeName) => No_issue_is_reported_for(@"
public enum " + typeName.ToUpperCaseAt(0) + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_value_as_name_only_on_([ValueSource(nameof(NonEnumTypes))] string type, [ValueSource(nameof(WrongTypeNames))] string typeName) => No_issue_is_reported_for(@"
public " + type + " " + typeName.ToUpperCaseAt(0) + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_enum_as_name_only_on_([ValueSource(nameof(AllowedEnumNames))] string suffix) => No_issue_is_reported_for(@"
public enum TestMe" + suffix + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_name_as_suffix_on_([ValueSource(nameof(NonEnumTypes))] string type, [ValueSource(nameof(WrongTypeNames))] string typeName) => An_issue_is_reported_for(@"
public " + type + " TestMe" + typeName.ToUpperCaseAt(0) + @"
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

        [Test]
        public void Code_gets_fixed_for_suffix_([ValueSource(nameof(NonEnumTypes))] string type, [ValueSource(nameof(WrongTypeNames))] string typeName)
            => VerifyCSharpFix(
                           "using System; " + type + " TestMe" + typeName.ToUpperCaseAt(0) + " { }",
                           "using System; " + type + " TestMe { }");

        [Test]
        public void Code_gets_fixed_for_enum_suffix_([ValueSource(nameof(WrongEnumNames))] string typeName) => VerifyCSharpFix(
                                                                                                                           "using System; enum TestMe" + typeName.ToUpperCaseAt(0) + " { }",
                                                                                                                           "using System; enum TestMe { }");

        [Test]
        public void Code_gets_fixed_for_Type_enum_with_Flags_([ValueSource(nameof(WrongEnumNames))] string typeName) => VerifyCSharpFix(
                                                                                                                                    "using System; [Flags] enum TestMe" + typeName.ToUpperCaseAt(0) + " { }",
                                                                                                                                    "using System; [Flags] enum TestMes { }");

        [Test]
        public void Code_gets_fixed_for_enum_ending_with_TypeEnum() => VerifyCSharpFix(
                                                                                   "using System; enum TestMeTypeEnum { }",
                                                                                   "using System; enum TestMeKind { }");

        [Test]
        public void Code_gets_fixed_for_Flags_enum_ending_with_TypeEnum() => VerifyCSharpFix(
                                                                                         "using System; [Flags] enum TestMeTypeEnum { }",
                                                                                         "using System; [Flags] enum TestMeKinds { }");

        protected override string GetDiagnosticId() => MiKo_1037_TypeSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1037_TypeSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1037_CodeFixProvider();
    }
}