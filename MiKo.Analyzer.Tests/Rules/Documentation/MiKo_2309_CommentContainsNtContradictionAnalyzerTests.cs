using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2309_CommentContainsNtContradictionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [TestCase("some comment")]
        [TestCase("some parent")]
        public void No_issue_is_reported_for_correct_single_line_comment_(string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // " + comment + @"
    }
}");

        [TestCase("some comment")]
        [TestCase("some parent")]
        public void No_issue_is_reported_for_correct_multi_line_comment_(string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        /* " + comment + @" */
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_single_line_comment() => Assert.Multiple(() =>
                                                                                                 {
                                                                                                     foreach (var delimiter in new[] { string.Empty, " ", ".", ",", ";", ":", "!", "?" })
                                                                                                     {
                                                                                                         foreach (var wrongPhrase in WrongContradictionPhrases)
                                                                                                         {
                                                                                                             An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // This " + wrongPhrase + delimiter + @"
    }
}");
                                                                                                         }
                                                                                                     }
                                                                                                 });

        [Test]
        public void An_issue_is_reported_for_wrong_multi_line_comment() => Assert.Multiple(() =>
                                                                                                {
                                                                                                    foreach (var delimiter in new[] { string.Empty, " ", ".", ",", ";", ":", "!", "?" })
                                                                                                    {
                                                                                                        foreach (var wrongPhrase in WrongContradictionPhrases)
                                                                                                        {
                                                                                                            An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        /* This " + wrongPhrase + delimiter + @" */
    }
}");
                                                                                                        }
                                                                                                    }
                                                                                                });

        [Test]
        public void Code_gets_fixed_for_single_line_([ValueSource(nameof(WrongContradictionPhrases))] string wrongPhrase)
        {
            const string Template = @"
public class TestMe
{
    public void DoSomething()
    {
        // ###
        DoSomething();
    }
}
";

            VerifyCSharpFix(Template.Replace("###", wrongPhrase), Template.Replace("###", ContradictionMap[wrongPhrase]));
        }

        [Test]
        public void Code_gets_fixed_for_single_multi_line_([ValueSource(nameof(WrongContradictionPhrases))] string wrongPhrase)
        {
            const string Template = @"
public class TestMe
{
    public void DoSomething()
    {
        /* ### */
        DoSomething();
    }
}
";

            VerifyCSharpFix(Template.Replace("###", wrongPhrase), Template.Replace("###", ContradictionMap[wrongPhrase]));
        }

        [Test]
        public void Code_gets_fixed_for_single_line_comment_on_end_of_line_([ValueSource(nameof(WrongContradictionPhrases))] string wrongPhrase)
        {
            const string Template = @"
public class TestMe
{
    public bool DoSomething()
    {
        return true; // ###
    }
}
";

            VerifyCSharpFix(Template.Replace("###", wrongPhrase), Template.Replace("###", ContradictionMap[wrongPhrase]));
        }

        [Test]
        public void Code_gets_fixed_for_multi_line_comment_on_end_of_line_([ValueSource(nameof(WrongContradictionPhrases))] string wrongPhrase)
        {
            const string Template = @"
public class TestMe
{
    public bool DoSomething()
    {
        return true; /* ### */
    }
}
";

            VerifyCSharpFix(Template.Replace("###", wrongPhrase), Template.Replace("###", ContradictionMap[wrongPhrase]));
        }

        protected override string GetDiagnosticId() => MiKo_2309_CommentContainsNtContradictionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2309_CommentContainsNtContradictionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2309_CodeFixProvider();
    }
}