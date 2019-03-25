using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2207_SummaryShallBeShortAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_being_not_too_long() => No_issue_is_reported_for(@"
/// <summary>
/// Specifies that the test fixture(s) marked with this attribute are considered to be <i>atomic</i> by NCrunch, meaning that their child tests cannot
/// be run separately from each other.
/// </summary>
/// <remarks>
/// A test being queued for execution under an atomic fixture will result in the entire fixture being queued with its child tests all executed in the
/// same task/batch.
/// </remarks>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_too_long_class_summary() => An_issue_is_reported_for(@"
/// <summary>
/// Specifies that the test fixture(s) marked with this attribute are considered to be <i>atomic</i> by NCrunch, meaning that their child tests cannot
/// be run separately from each other.
/// A test being queued for execution under an atomic fixture will result in the entire fixture being queued with its child tests all executed in the
/// same task/batch.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_containing_minus_sign_and_has_exact_limit() => No_issue_is_reported_for(@"
/// <summary>
/// A b c d e f g h i j k l m n o p q r t u v w x y z - 1 2 3 4 5 6 7 8 9 0 - A b c d e f g h i j k l m n o p. 
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_summary_containing_minus_sign_and_has_exactly_1_word_more_than_limit() => An_issue_is_reported_for(@"
/// <summary>
/// A b c d e f g h i j k l m n o p q r t u v w x y z - 1 2 3 4 5 6 7 8 9 0 - A b c d e f g h i j k l m n o p q. 
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_containing_see_cref_and_has_exact_limit() => No_issue_is_reported_for(@"
/// <summary>
/// A b c d e f g h i j k l m n o p q r t u v w x y z <see cref=""TestMe"" /> 1 2 3 4 5 6 7 8 9 0 <see cref=""TestMe"" /> A b c d e f g h i j k l m n. 
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_summary_containing_see_cref_and_has_exactly_1_word_more_than_limit() => An_issue_is_reported_for(@"
/// <summary>
/// A b c d e f g h i j k l m n o p q r t u v w x y z <see cref=""TestMe"" /> 1 2 3 4 5 6 7 8 9 0 <see cref=""TestMe"" /> A b c d e f g h i j k l m n o. 
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_summary_containing_see_langword_and_has_exact_limit() => No_issue_is_reported_for(@"
/// <summary>
/// A b c d e f g h i j k l m n o p q r t u v w x y z <see langword=""null"" /> 1 2 3 4 5 6 7 8 9 0 <see langword=""null"" /> A b c d e f g h i j k l m n. 
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_summary_containing_see_langword_and_has_exactly_1_word_more_than_limit() => An_issue_is_reported_for(@"
/// <summary>
/// A b c d e f g h i j k l m n o p q r t u v w x y z <see langword=""null"" /> 1 2 3 4 5 6 7 8 9 0 <see langword=""null"" /> A b c d e f g h i j k l m n o. 
/// </summary>
public class TestMe
{
}
");

        protected override string GetDiagnosticId() => MiKo_2207_SummaryShallBeShortAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2207_SummaryShallBeShortAnalyzer();
    }
}