using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6057_TypeParameterConstraintClausesVerticallyAlignedAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2, T3>()
                                where T1 : class
                                where T2 : class
                                where T3 : class
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
        public void No_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_([ValueSource(nameof(Types))] string type) => No_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2, T3>
                                where T1 : class
                                where T2 : class
                                where T3 : class
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_method_with_type_parameter_constraint_clauses_aligned_horizontally() => An_issue_is_reported_for(2, @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class where T2 : class where T3 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                      where T2 : class
                                          where T3 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                              where T2 : class
                                          where T3 : class
    { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_left_and_another_more_to_right() => An_issue_is_reported_for(2, @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                      where T2 : class
                                              where T3 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class

                                          where T2 : class
                                          where T3 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_and_is_aligned_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class

                                      where T2 : class
                                          where T3 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_and_is_aligned_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class

                                              where T2 : class
                                          where T3 : class
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_left_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2, T3>
                                where T1 : class
                            where T2 : class
                                where T3 : class
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_horizontally_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(2, @"
public " + type + @" TestMe<T1, T2, T3> where T1 : class where T2 : class where T3 : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_right_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2, T3>
                                    where T1 : class
                                        where T2 : class
                                    where T3 : class
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_left_and_another_more_to_right_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(2, @"
public " + type + @" TestMe<T1, T2, T3>
                                    where T1 : class
                                where T2 : class
                                        where T3 : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2, T3>
                                    where T1 : class

                                    where T2 : class
                                    where T3 : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_and_is_aligned_more_to_left_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2, T3>
                                    where T1 : class

                                where T2 : class
                                    where T3 : class
{
}
");

        [Test]
        public void An_issue_is_reported_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_and_is_aligned_more_to_right_([ValueSource(nameof(Types))] string type) => An_issue_is_reported_for(@"
public " + type + @" TestMe<T1, T2, T3>
                                    where T1 : class

                                        where T2 : class
                                    where T3 : class
{
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_type_parameter_constraint_clauses_aligned_horizontally()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class where T2 : class where T3 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                          where T2 : class
                                          where T3 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                      where T2 : class
                                          where T3 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                          where T2 : class
                                          where T3 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                              where T2 : class
                                          where T3 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                          where T2 : class
                                          where T3 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_left_and_another_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                      where T2 : class
                                              where T3 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                          where T2 : class
                                          where T3 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class

                                          where T2 : class
                                          where T3 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                          where T2 : class
                                          where T3 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_and_is_aligned_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class

                                      where T2 : class
                                          where T3 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                          where T2 : class
                                          where T3 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_and_is_aligned_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class

                                              where T2 : class
                                          where T3 : class
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething<T1, T2, T3>() where T1 : class
                                          where T2 : class
                                          where T3 : class
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_type_with_multiple_type_parameter_constraint_clauses_aligned_horizontally_([ValueSource(nameof(Types))] string type)
        {
            const string OriginalCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class where T2 : class where T3 : class
{
}
";

            const string FixedCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                                where T2 : class
                                where T3 : class
{
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_left_([ValueSource(nameof(Types))] string type)
        {
            const string OriginalCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                            where T2 : class
                                where T3 : class
{
}
";

            const string FixedCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                                where T2 : class
                                where T3 : class
{
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_right_([ValueSource(nameof(Types))] string type)
        {
            const string OriginalCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                                    where T2 : class
                                where T3 : class
{
}
";

            const string FixedCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                                where T2 : class
                                where T3 : class
{
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_aligned_more_to_left_and_another_more_to_right_([ValueSource(nameof(Types))] string type)
        {
            const string OriginalCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                            where T2 : class
                                    where T3 : class
{
}
";

            const string FixedCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                                where T2 : class
                                where T3 : class
{
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_([ValueSource(nameof(Types))] string type)
        {
            const string OriginalCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class

                                where T2 : class
                                where T3 : class
{
}
";

            const string FixedCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                                where T2 : class
                                where T3 : class
{
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_and_is_aligned_more_to_left_([ValueSource(nameof(Types))] string type)
        {
            const string OriginalCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class

                            where T2 : class
                                where T3 : class
{
}
";

            const string FixedCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                                where T2 : class
                                where T3 : class
{
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_type_with_multiple_type_parameter_constraint_clauses_aligned_vertically_and_one_has_empty_line_between_and_is_aligned_more_to_right_([ValueSource(nameof(Types))] string type)
        {
            const string OriginalCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class

                                    where T2 : class
                                where T3 : class
{
}
";

            const string FixedCode = @"
public ### TestMe<T1, T2, T3>
                                where T1 : class
                                where T2 : class
                                where T3 : class
{
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        protected override string GetDiagnosticId() => MiKo_6057_TypeParameterConstraintClausesVerticallyAlignedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6057_TypeParameterConstraintClausesVerticallyAlignedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6057_CodeFixProvider();
    }
}