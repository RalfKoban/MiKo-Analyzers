using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3078_EnumMembersHaveValueAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_class_with_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int myField;
}
");

        [Test]
        public void No_issue_is_reported_for_enum_fields_with_values() => No_issue_is_reported_for(@"
using System;

public enum TestMe
{
    None = 0,
    Something = 1,
    Anything =2,
}
");

        [Test]
        public void An_issue_is_reported_for_enum_field_without_value() => An_issue_is_reported_for(@"
using System;

public enum TestMe
{
    None = 0,
    Something,
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_each_enum_field_without_value() => An_issue_is_reported_for(3, @"
using System;

public enum TestMe
{
    None,
    Something,
    Anything,
}
");

        [Test]
        public void Code_gets_fixed_for_each_enum_field_without_value()
        {
            const string OriginalCode = @"
using System;

public enum TestMe
{
    None,
    Something,
    Anything,
    More,
    EvenMore,
    StillMore,
}
";

            const string FixedCode = @"
using System;

public enum TestMe
{
    None = 0,
    Something = 1,
    Anything = 2,
    More = 3,
    EvenMore = 4,
    StillMore = 5,
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_each_enum_field_without_value_on_Flags_enum()
        {
            const string OriginalCode = @"
using System;

[Flags]
public enum TestMe
{
    None,
    Something,
    Anything,
    More,
    EvenMore,
    StillMore,
}
";

            const string FixedCode = @"
using System;

[Flags]
public enum TestMe
{
    None = 0,
    Something = 1 << 0,
    Anything = 1 << 1,
    More = 1 << 2,
    EvenMore = 1 << 3,
    StillMore = 1 << 4,
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase(0, ExpectedResult = 1)]
        [TestCase(1, ExpectedResult = 2)]
        [TestCase(2, ExpectedResult = 4)]
        [TestCase(3, ExpectedResult = 8)]
        [TestCase(4, ExpectedResult = 16)]
        [TestCase(5, ExpectedResult = 32)]
        public int Left_shifting_One_leads_to_correct_value_for_(int i) => 1 << i;

        protected override string GetDiagnosticId() => MiKo_3078_EnumMembersHaveValueAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3078_EnumMembersHaveValueAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3078_CodeFixProvider();
    }
}