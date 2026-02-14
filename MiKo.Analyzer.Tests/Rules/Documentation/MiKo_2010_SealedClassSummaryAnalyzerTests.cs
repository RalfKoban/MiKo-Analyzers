using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2010_SealedClassSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_struct() => No_issue_is_reported_for(@"
/// <summary>
/// Something.
/// </summary>
public struct TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_unsealed_class() => No_issue_is_reported_for(@"
/// <summary>
/// Something.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_public_sealed_class() => No_issue_is_reported_for(@"
/// <summary>
/// Something.
/// </summary>
private sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_sealed_class() => No_issue_is_reported_for(@"
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_sealed_class_with_inheritance_statement() => No_issue_is_reported_for(@"
/// <summary>
/// This class cannot be inherited.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_sealed_class_without_inheritance_statement() => An_issue_is_reported_for(@"
/// <summary>
/// Some documentation
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_sealed_test_class_without_inheritance_statement_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
/// <summary>
/// Some documentation
/// </summary>
[" + fixture + @"]
public sealed class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_sealed_class_with_misplaced_inheritance_statement() => An_issue_is_reported_for(@"
/// <summary>
/// This class cannot be inherited.
/// Some documentation
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Code_gets_fixed_by_adding_inheritance_statement_at_end_of_summary()
        {
            const string OriginalCode = @"
/// <summary>
/// Some documentation
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some documentation
/// This class cannot be inherited.
/// </summary>
public sealed class TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_adding_inheritance_statement_at_end_of_summary_for_record()
        {
            const string OriginalCode = @"
/// <summary>
/// Some documentation
/// </summary>
public sealed record TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some documentation
/// This class cannot be inherited.
/// </summary>
public sealed record TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_moving_inheritance_statement_to_end_of_summary()
        {
            const string OriginalCode = @"
/// <summary>
/// This class cannot be inherited.
/// Some documentation.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some documentation.
/// This class cannot be inherited.
/// </summary>
public sealed class TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_moving_inheritance_statement_from_middle_of_line_to_end()
        {
            const string OriginalCode = @"
/// <summary>
/// Some text. This class cannot be inherited.
/// Some documentation.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some text. 
/// Some documentation.
/// This class cannot be inherited.
/// </summary>
public sealed class TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_by_extracting_and_moving_inheritance_statement_to_end()
        {
            const string OriginalCode = @"
/// <summary>
/// Some text. This class cannot be inherited. Some documentation.
/// </summary>
public sealed class TestMe
{
}
";

            const string FixedCode = @"
/// <summary>
/// Some text. Some documentation.
/// This class cannot be inherited.
/// </summary>
public sealed class TestMe
{
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2010_SealedClassSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2010_SealedClassSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2010_CodeFixProvider();
    }
}