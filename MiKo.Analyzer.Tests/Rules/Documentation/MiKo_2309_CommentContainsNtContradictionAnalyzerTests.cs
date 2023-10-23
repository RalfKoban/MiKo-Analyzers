using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2309_CommentContainsNtContradictionAnalyzerTests : CodeFixVerifier
    {
        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
                                                                     {
                                                                         { "aren't", "are not" },
                                                                         { "can't", "cannot" },
                                                                         { "couldn't", "could not" },
                                                                         { "daren't", "dare not" },
                                                                         { "didn't", "did not" },
                                                                         { "doesn't", "does not" },
                                                                         { "don't", "do not" },
                                                                         { "hadn't", "had not" },
                                                                         { "hasn't", "has not" },
                                                                         { "haven't", "have not" },
                                                                         { "isn't", "is not" },
                                                                         { "needn't", "need not" },
                                                                         { "shouldn't", "should not" },
                                                                         { "wasn't", "was not" },
                                                                         { "weren't", "were not" },
                                                                         { "won't", "will not" },
                                                                         { "wouldn't", "would not" },

                                                                         // capitalized
                                                                         { "Aren't", "Are not" },
                                                                         { "Can't", "Cannot" },
                                                                         { "Couldn't", "Could not" },
                                                                         { "Daren't", "Dare not" },
                                                                         { "Didn't", "Did not" },
                                                                         { "Doesn't", "Does not" },
                                                                         { "Don't", "Do not" },
                                                                         { "Hadn't", "Had not" },
                                                                         { "Hasn't", "Has not" },
                                                                         { "Haven't", "Have not" },
                                                                         { "Isn't", "Is not" },
                                                                         { "Needn't", "Need not" },
                                                                         { "Shouldn't", "Should not" },
                                                                         { "Wasn't", "Was not" },
                                                                         { "Weren't", "Were not" },
                                                                         { "Won't", "Will not" },
                                                                         { "Wouldn't", "Would not" },

                                                                         // without apostrophes
                                                                         { "cant", "cannot" },
                                                                         { "couldnt", "could not" },
                                                                         { "darent", "dare not" },
                                                                         { "didnt", "did not" },
                                                                         { "doesnt", "does not" },
                                                                         { "dont", "do not" },
                                                                         { "hadnt", "had not" },
                                                                         { "hasnt", "has not" },
                                                                         { "havent", "have not" },
                                                                         { "isnt", "is not" },
                                                                         { "neednt", "need not" },
                                                                         { "shouldnt", "should not" },
                                                                         { "wasnt", "was not" },
                                                                         { "werent", "were not" },
                                                                         { "wont", "will not" },
                                                                         { "wouldnt", "would not" },

                                                                         // capitalized without apostrophes
                                                                         { "Cant", "Cannot" },
                                                                         { "Couldnt", "Could not" },
                                                                         { "Darent", "Dare not" },
                                                                         { "Didnt", "Did not" },
                                                                         { "Doesnt", "Does not" },
                                                                         { "Dont", "Do not" },
                                                                         { "Hadnt", "Had not" },
                                                                         { "Hasnt", "Has not" },
                                                                         { "Havent", "Have not" },
                                                                         { "Isnt", "Is not" },
                                                                         { "Neednt", "Need not" },
                                                                         { "Shouldnt", "Should not" },
                                                                         { "Wasnt", "Was not" },
                                                                         { "Werent", "Were not" },
                                                                         { "Wont", "Will not" },
                                                                         { "Wouldnt", "Would not" },
                                                                     };

        private static readonly string[] WrongPhrases = Map.Keys.ToArray();

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
                                                                                                         foreach (var wrongPhrase in WrongPhrases)
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
                                                                                                        foreach (var wrongPhrase in WrongPhrases)
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
        public void Code_gets_fixed_for_single_line_([ValueSource(nameof(WrongPhrases))] string wrongPhrase)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // " + wrongPhrase + @"
        DoSomething();
    }
}
";

            var fixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // " + Map[wrongPhrase] + @"
        DoSomething();
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_single_multi_line_([ValueSource(nameof(WrongPhrases))] string wrongPhrase)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        /* " + wrongPhrase + @" */
        DoSomething();
    }
}
";

            var fixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        /* " + Map[wrongPhrase] + @" */
        DoSomething();
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_single_line_comment_on_end_of_line_([ValueSource(nameof(WrongPhrases))] string wrongPhrase)
        {
            var originalCode = @"
public class TestMe
{
    public bool DoSomething()
    {
        return true; // " + wrongPhrase + @"
    }
}
";

            var fixedCode = @"
public class TestMe
{
    public bool DoSomething()
    {
        return true; // " + Map[wrongPhrase] + @"
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multi_line_comment_on_end_of_line_([ValueSource(nameof(WrongPhrases))] string wrongPhrase)
        {
            var originalCode = @"
public class TestMe
{
    public bool DoSomething()
    {
        return true; /* " + wrongPhrase + @" */
    }
}
";

            var fixedCode = @"
public class TestMe
{
    public bool DoSomething()
    {
        return true; /* " + Map[wrongPhrase] + @" */
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2309_CommentContainsNtContradictionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2309_CommentContainsNtContradictionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2309_CodeFixProvider();
    }
}