using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6058_TypeParameterConstraintClauseIndentedBelowParameterListAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Types = ["class", "interface", "record", "struct"];

        [Test]
        public void No_issue_is_reported_for_method_without_type_parameter_constraint_clause() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_type_parameter_constraint_clause_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>() where T : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2>() where T1 : class where T2 : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_type_parameter_constraint_clause_aligned_with_closing_parenthesis() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>()
                            where T : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_aligned_with_closing_parenthesis() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2>()
                                 where T1 : class
                                 where T2 : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_without_type_parameter_constraint_clause() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything()
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_type_parameter_constraint_clause_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T>() where T : class
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_multiple_type_parameter_constraint_clauses_on_same_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T1, T2>() where T1 : class where T2 : class
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_type_parameter_constraint_clause_aligned_with_closing_parenthesis() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T>()
                        where T : class
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_multiple_type_parameter_constraint_clauses_aligned_with_closing_parenthesis() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T1, T2>()
                             where T1 : class
                             where T2 : class
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_type_without_type_parameter_constraint_clause_([ValueSource(nameof(Types))] string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_type_parameter_constraint_clause_on_same_line_([ValueSource(nameof(Types))] string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe<T> where T : class
{
}
");

        [Test]
        public void No_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_on_same_line_([ValueSource(nameof(Types))] string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2, T3> where T1 : class where T2 : class where T3 : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_type_parameter_constraint_clause_indented_too_far_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>()
        where T : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_type_parameter_constraint_clause_indented_too_far_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>()
                                 where T : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_indented_too_far_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2>()
        where T1 : class
        where T2 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_indented_too_far_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2>()
                                      where T1 : class
                                      where T2 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_type_parameter_constraint_clause_indented_too_far_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T>()
            where T : class
        { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_type_parameter_constraint_clause_indented_too_far_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T>()
                             where T : class
        { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_multiple_type_parameter_constraint_clauses_indented_too_far_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T1, T2>()
            where T1 : class
            where T2 : class
        { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_multiple_type_parameter_constraint_clauses_indented_too_far_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T1, T2>()
                                    where T1 : class
                                    where T2 : class
        { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_type_parameter_constraint_clause_indented_too_far_left_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T>
    where T : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_type_parameter_constraint_clause_indented_too_far_right_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T>
                                    where T : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_indented_too_far_left_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2>
    where T1 : class
    where T2 : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_indented_too_far_right_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2>
                                          where T1 : class
                                          where T2 : class
{
}
");

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_parenthesis_when_indented_too_far_left_for_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T>()
        where T : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T>()
                            where T : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_parenthesis_when_indented_too_far_right_for_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T>()
                                 where T : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T>()
                            where T : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_parenthesis_when_indented_too_far_left_for_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2>()
        where T1 : class
        where T2 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2>()
                                 where T1 : class
        where T2 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_parenthesis_when_indented_too_far_right_for_method()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2>()
                                      where T1 : class
                                      where T2 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2>()
                                 where T1 : class
                                      where T2 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_parenthesis_when_indented_too_far_left_for_local_function()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T>()
            where T : class
        { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T>()
                        where T : class
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_parenthesis_when_indented_too_far_right_for_local_function()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T>()
                                where T : class
        { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T>()
                        where T : class
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_parenthesis_when_indented_too_far_left_for_local_function()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T1, T2>()
            where T1 : class
            where T2 : class
        { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T1, T2>()
                             where T1 : class
            where T2 : class
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_parenthesis_when_indented_too_far_right_for_local_function()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T1, T2>()
                                    where T1 : class
                                    where T2 : class
        { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        void DoAnything<T1, T2>()
                             where T1 : class
                                    where T2 : class
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_angle_bracket_when_indented_too_far_left_for_class()
        {
            const string OriginalCode = @"
public class TestMe<T>
    where T : class
{
}
";

            const string FixedCode = @"
public class TestMe<T>
                  where T : class
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_angle_bracket_when_indented_too_far_right_for_class()
        {
            const string OriginalCode = @"
public class TestMe<T>
                                    where T : class
{
}
";

            const string FixedCode = @"
public class TestMe<T>
                  where T : class
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_angle_bracket_when_indented_too_far_left_for_class()
        {
            const string OriginalCode = @"
public class TestMe<T1, T2>
    where T1 : class
    where T2 : class
{
}
";

            const string FixedCode = @"
public class TestMe<T1, T2>
                       where T1 : class
    where T2 : class
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_angle_bracket_when_indented_too_far_right_for_class()
        {
            const string OriginalCode = @"
public class TestMe<T1, T2>
                              where T1 : class
                              where T2 : class
{
}
";

            const string FixedCode = @"
public class TestMe<T1, T2>
                       where T1 : class
                              where T2 : class
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_angle_bracket_when_indented_too_far_left_for_interface()
        {
            const string OriginalCode = @"
public interface TestMe<T>
    where T : class
{
}
";

            const string FixedCode = @"
public interface TestMe<T>
                      where T : class
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_angle_bracket_when_indented_too_far_right_for_interface()
        {
            const string OriginalCode = @"
public interface TestMe<T>
                                where T : class
{
}
";

            const string FixedCode = @"
public interface TestMe<T>
                      where T : class
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_angle_bracket_when_indented_too_far_left_for_interface()
        {
            const string OriginalCode = @"
public interface TestMe<T1, T2>
    where T1 : class
    where T2 : class
{
}
";

            const string FixedCode = @"
public interface TestMe<T1, T2>
                           where T1 : class
    where T2 : class
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_angle_bracket_when_indented_too_far_right_for_interface()
        {
            const string OriginalCode = @"
public interface TestMe<T1, T2>
                              where T1 : class
                              where T2 : class
{
}
";

            const string FixedCode = @"
public interface TestMe<T1, T2>
                           where T1 : class
                              where T2 : class
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_angle_bracket_when_indented_too_far_left_for_record()
        {
            const string OriginalCode = @"
public record TestMe<T>
    where T : class
{
}
";

            const string FixedCode = @"
public record TestMe<T>
                   where T : class
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_angle_bracket_when_indented_too_far_right_for_record()
        {
            const string OriginalCode = @"
public record TestMe<T>
                                    where T : class
{
}
";

            const string FixedCode = @"
public record TestMe<T>
                   where T : class
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_angle_bracket_when_indented_too_far_left_for_record()
        {
            const string OriginalCode = @"
public record TestMe<T1, T2>
    where T1 : class
    where T2 : class
{
}
";

            const string FixedCode = @"
public record TestMe<T1, T2>
                        where T1 : class
    where T2 : class
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_angle_bracket_when_indented_too_far_right_for_record()
        {
            const string OriginalCode = @"
public record TestMe<T1, T2>
                              where T1 : class
                              where T2 : class
{
}
";

            const string FixedCode = @"
public record TestMe<T1, T2>
                        where T1 : class
                              where T2 : class
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_angle_bracket_when_indented_too_far_left_for_struct()
        {
            const string OriginalCode = @"
public struct TestMe<T>
    where T : class
{
}
";

            const string FixedCode = @"
public struct TestMe<T>
                   where T : class
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_constraint_clause_with_closing_angle_bracket_when_indented_too_far_right_for_struct()
        {
            const string OriginalCode = @"
public struct TestMe<T>
                                    where T : class
{
}
";

            const string FixedCode = @"
public struct TestMe<T>
                   where T : class
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_angle_bracket_when_indented_too_far_left_for_struct()
        {
            const string OriginalCode = @"
public struct TestMe<T1, T2>
    where T1 : class
    where T2 : class
{
}
";

            const string FixedCode = @"
public struct TestMe<T1, T2>
                        where T1 : class
    where T2 : class
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_align_first_constraint_clause_with_closing_angle_bracket_when_indented_too_far_right_for_struct()
        {
            const string OriginalCode = @"
public struct TestMe<T1, T2>
                              where T1 : class
                              where T2 : class
{
}
";

            const string FixedCode = @"
public struct TestMe<T1, T2>
                        where T1 : class
                              where T2 : class
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6058_TypeParameterConstraintClauseIndentedBelowParameterListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6058_TypeParameterConstraintClauseIndentedBelowParameterListAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6058_CodeFixProvider();
    }
}