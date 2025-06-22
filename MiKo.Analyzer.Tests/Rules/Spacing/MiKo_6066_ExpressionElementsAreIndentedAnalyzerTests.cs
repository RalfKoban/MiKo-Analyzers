using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6066_ExpressionElementsAreIndentedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_field_with_collection_expression_on_single_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [1, 2, 3];
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_collection_expression_with_correctly_indented_elements() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [
                                1,
                                2,
                                3
                            ];
}
");

        [Test]
        public void No_issue_is_reported_for_field_with_collection_expression_with_correctly_indented_elements_and_comments() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [
                                // First element
                                1,
                                // Second element
                                2,
                                3
                            ];
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_collection_expression_with_element_on_same_position_like_bracket() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [
                            1,
                                2,
                                3
                            ];
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_collection_expression_with_outdented_element() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = [
                        1,
                                2,
                                3
                            ];
}
");

        [Test]
        public void Code_gets_fixed_for_field_with_collection_expression_with_element_on_same_position_like_bracket()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                            1,
                                2,
                                3
                            ];
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                                1,
                                2,
                                3
                            ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_with_collection_expression_with_element_on_same_position_like_bracket_and_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                            // some comment
                            1,
                                2,
                                3
                            ];
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                                // some comment
                                1,
                                2,
                                3
                            ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_with_collection_expression_with_outdented_element()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                        1,
                                2,
                                3
                            ];
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                                1,
                                2,
                                3
                            ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_with_collection_expression_with_all_elements_outdented()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                        1,
                        2,
                        3
                            ];
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = [
                                1,
                                2,
                                3
                            ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6066_ExpressionElementsAreIndentedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6066_ExpressionElementsAreIndentedAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6066_CodeFixProvider();
    }
}