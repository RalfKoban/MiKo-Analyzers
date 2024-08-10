using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6058_TypeParameterConstraintClauseIndentedBelowTypeParameterListAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_on_same_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2>() where T1 : class where T2 : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_type_parameter_constraint_clause_properly_indented_on_different_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>()
                             where T : class
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_properly_indented_on_different_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2>()
                             where T1 : class
                             where T2 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>()
        where T : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T>()
                                 where T : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2>()
        where T1 : class
        where T2 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2>()
                                      where T1 : class
                                      where T2 : class
    { }
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
        public void No_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_horizontally_([ValueSource(nameof(Types))] string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2, T3> where T1 : class where T2 : class where T3 : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_line_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T>
    where T : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_line_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T>
                                    where T : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_lines_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2>
    where T1 : class
    where T2 : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_lines_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2>
                                          where T1 : class
                                          where T2 : class
{
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_line()
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
        public void Code_gets_fixed_for_method_with_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_line()
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
        public void Code_gets_fixed_for_method_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_lines()
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
        public void Code_gets_fixed_for_method_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_lines()
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
        public void Code_gets_fixed_for_class_with_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_line()
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
        public void Code_gets_fixed_for_class_with_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_line()
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
        public void Code_gets_fixed_for_class_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_lines()
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
        public void Code_gets_fixed_for_class_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_lines()
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
        public void Code_gets_fixed_for_interface_with_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_line()
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
        public void Code_gets_fixed_for_interface_with_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_line()
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
        public void Code_gets_fixed_for_interface_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_lines()
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
        public void Code_gets_fixed_for_interface_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_lines()
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
        public void Code_gets_fixed_for_record_with_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_line()
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
        public void Code_gets_fixed_for_record_with_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_line()
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
        public void Code_gets_fixed_for_record_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_lines()
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
        public void Code_gets_fixed_for_record_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_lines()
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
        public void Code_gets_fixed_for_struct_with_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_line()
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
        public void Code_gets_fixed_for_struct_with_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_line()
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
        public void Code_gets_fixed_for_struct_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_left_on_different_lines()
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
        public void Code_gets_fixed_for_struct_with_multiple_type_parameter_constraint_clause_incorrectly_indented_to_right_on_different_lines()
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

        protected override string GetDiagnosticId() => MiKo_6058_TypeParameterConstraintClauseIndentedBelowTypeParameterListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6058_TypeParameterConstraintClauseIndentedBelowTypeParameterListAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6058_CodeFixProvider();
    }
}