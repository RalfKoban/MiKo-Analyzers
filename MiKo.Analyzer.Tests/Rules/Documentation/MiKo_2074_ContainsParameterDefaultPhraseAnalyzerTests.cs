﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2074_ContainsParameterDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] MethodNames = ["Contains", "ContainsKey"];

        private static readonly string[] CorrectComments = [
                                                               "Something to seek",
                                                               "Something to seek.",
                                                               "Something to seek in the list.",
                                                               "Something to seek in the array.",
                                                               "Something to seek in the collection.",
                                                               "Something to locate",
                                                               "Something to locate.",
                                                               "Something to locate in the list.",
                                                               "Something to locate in the array.",
                                                               "Something to locate in the collection.",
                                                           ];

        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public bool Contains()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_documented_method_([ValueSource(nameof(MethodNames))] string methodName, [ValueSource(nameof(CorrectComments))] string comment)
            => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// " + comment + @"
    /// </param>
    public bool " + methodName + @"(int i)
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_documented_method_with_multiple_parameters_([ValueSource(nameof(MethodNames))] string methodName, [ValueSource(nameof(CorrectComments))] string comment)
            => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// " + comment + @"
    /// </param>
    /// <param name=""j"">
    /// Something to consider.
    /// </param>
    public bool " + methodName + @"(int i, int j)
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_documented_extension_method_([ValueSource(nameof(MethodNames))] string methodName, [ValueSource(nameof(CorrectComments))] string comment)
            => No_issue_is_reported_for(@"
public static class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""array"">
    /// The array to search.
    /// </param>
    /// <param name=""i"">
    /// " + comment + @"
    /// </param>
    public static bool " + methodName + @"(this int[] array, int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_documented_non_Contains_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// Something to find.
    /// </param>
    public void DoSomething(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_method_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// Something to find.
    /// </param>
    public bool " + methodName + @"(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_first_parameter_of_incorrectly_documented_method_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// Something to find.
    /// </param>
    /// <param name=""j"">
    /// Something to consider.
    /// </param>
    public bool " + methodName + @"(int i, int j)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_second_parameter_of_incorrectly_documented_extension_method_([ValueSource(nameof(MethodNames))] string methodName) => An_issue_is_reported_for(@"
public static class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""array"">
    /// The array to search.
    /// </param>
    /// <param name=""i"">
    /// Something to find.
    /// </param>
    public static bool " + methodName + @"(this int[] array, int i)
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_empty_parameter_on_method_(
                                                                    [ValueSource(nameof(MethodNames))] string methodName,
                                                                    [Values(@"<param name=""i""></param>", @"<param name=""i"">     </param>")] string parameter)
            => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// " + parameter + @"
    public bool " + methodName + @"(int i)
    {
    }
}
");

        [TestCase("The item", "The item")]
        [TestCase("The value to search", "The value")]
        [TestCase("The value to search for", "The value")]
        public void Code_gets_fixed_for_simple_text(string originalStartingPhrase, string fixedStartingPhrase)
        {
            var originalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// " + originalStartingPhrase + @".
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            var fixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// " + fixedStartingPhrase + @" to seek.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_text_with_seeCref_and_ending_dot()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The <see cref=""int""/>.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The <see cref=""int""/> to seek.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_text_with_seeCref_without_ending_dot()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The <see cref=""int""/>
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">
    /// The <see cref=""int""/> to seek.
    /// </param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_text_with_seeCref_on_same_line_without_ending_dot()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">The <see cref=""int""/></param>
    public bool Contains(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">The <see cref=""int""/> to seek.</param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_empty_text()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i""></param>
    public bool Contains(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <param name=""i"">The item to seek.</param>
    public bool Contains(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2074_ContainsParameterDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2074_ContainsParameterDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2074_CodeFixProvider();
    }
}