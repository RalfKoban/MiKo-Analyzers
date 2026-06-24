using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6069_ObjectInitializerExpressionsAreIndentedBesideOpenBraceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_initializer_expression_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe { Number = 42 };
}
");

        [Test]
        public void No_issue_is_reported_for_initializer_expression_on_other_line_but_indented() => No_issue_is_reported_for(@"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                      Number = 42,
                                  };
}
");

        [Test]
        public void An_issue_is_reported_for_initializer_expression_on_other_line_but_more_indented() => An_issue_is_reported_for(@"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                          Number = 42,
                                  };
}
");

        [Test]
        public void An_issue_is_reported_for_initializer_expression_on_other_line_but_outdented() => An_issue_is_reported_for(@"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                  Number = 42,
                                  };
}
");

        [Test]
        public void Code_gets_fixed_for_initializer_expression_on_other_line_but_outdented()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                          Number = 42,
                                  };
}
";

            const string FixedCode = @"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                      Number = 42,
                                  };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_initializer_expression_on_other_line_but_on_same_position_as_open_brace()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                  Number = 42,
                                  };
}
";

            const string FixedCode = @"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                      Number = 42,
                                  };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_initializer_expressions_on_other_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Name { get; set; }

    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                        Name = ""my name"",
                                  Number = 42,
                                  };
}
";

            const string FixedCode = @"
public class TestMe
{
    public int Name { get; set; }

    public int Number { get; set; }

    public static Create() => new TestMe
                                  {
                                      Name = ""my name"",
                                      Number = 42,
                                  };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_initializer_expression_on_same_line_but_on_position_0_character_behind_open_brace()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe {Number = 42 };
}
";

            const string FixedCode = @"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe { Number = 42 };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_initializer_expression_on_same_line_but_on_position_more_than_1_character_behind_open_brace()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe {   Number = 42 };
}
";

            const string FixedCode = @"
public class TestMe
{
    public int Number { get; set; }

    public static Create() => new TestMe { Number = 42 };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_initializer_expressions_on_same_line_but_with_additional_spaces()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Name { get; set; }

    public int Number { get; set; }

    public static Create() => new TestMe {   Number = 42,      Name = ""some name"" };
}
";

            const string FixedCode = @"
public class TestMe
{
    public int Name { get; set; }

    public int Number { get; set; }

    public static Create() => new TestMe { Number = 42, Name = ""some name"" };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_initializer_expressions_on_same_line_but_with_no_spaces()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Name { get; set; }

    public int Number { get; set; }

    public static Create() => new TestMe {Number = 42,Name = ""some name"" };
}
";

            const string FixedCode = @"
public class TestMe
{
    public int Name { get; set; }

    public int Number { get; set; }

    public static Create() => new TestMe { Number = 42, Name = ""some name"" };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6069_ObjectInitializerExpressionsAreIndentedBesideOpenBraceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6069_ObjectInitializerExpressionsAreIndentedBesideOpenBraceAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6069_CodeFixProvider();
    }
}