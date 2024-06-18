using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] BooleanOnlyReturnValues =
                                                                   [
                                                                       "bool",
                                                                       "Boolean",
                                                                       "System.Boolean",
                                                                   ];

        private static readonly string[] BooleanTaskReturnValues =
                                                                   [
                                                                       "Task<bool>",
                                                                       "Task<Boolean>",
                                                                       "Task<System.Boolean>",
                                                                       "System.Threading.Tasks.Task<bool>",
                                                                       "System.Threading.Tasks.Task<Boolean>",
                                                                       "System.Threading.Tasks.Task<System.Boolean>",
                                                                   ];

        private static readonly string[] SimpleStartingPhrases = MiKo_2032_CodeFixProvider.CreateSimpleStartingPhrases().ToArray();

        private static readonly string[] BooleanReturnValues = [.. BooleanOnlyReturnValues, .. BooleanTaskReturnValues];

        [Test]
        public void No_issue_is_reported_for_uncommented_method_([ValueSource(nameof(BooleanReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property_([ValueSource(nameof(BooleanReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a_(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("void", "int", "Task", "Task<int>", "Task<string>")] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// Something.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => throw new NotSupportedException();
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method_(
                                                                                  [Values("returns", "value")] string xmlTag,
                                                                                  [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                  [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                  [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + " if something happens; otherwise, " + falseValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method_with_default_phrase_(
                                                                                                      [Values("returns", "value")] string xmlTag,
                                                                                                      [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                                      [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                                      [Values("<see langword=\"true\"/>", "<see langword=\"false\"/>")] string defaultValue,
                                                                                                      [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + " if something happens; otherwise, " + falseValue + ". The default is " + defaultValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method_with_line_break_(
                                                                                                  [Values("returns", "value")] string xmlTag,
                                                                                                  [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                                  [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                                  [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + @" if something happens;
    /// otherwise, " + falseValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_Task_method_(
                                                                                  [Values("returns", "value")] string xmlTag,
                                                                                  [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                  [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                  [ValueSource(nameof(BooleanTaskReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A task that will complete with a result of " + trueValue + " if something happens, otherwise with a result of " + falseValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_property_with_To_and_setter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Gets or sets a value.
    /// </summary>
    /// <value>
    /// <see langword=""true""/> to set something; otherwise, <see langword=""false""/>.
    /// </value>
    public bool SomeProperty { get; set; }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_method_(
                                                                 [Values("returns", "value")] string xmlTag,
                                                                 [Values("A whatever", "An whatever", "The whatever")] string comment,
                                                                 [ValueSource(nameof(BooleanReturnValues))] string returnType)
            => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + comment + @"
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_commented_method_with_starting_phrase_([ValueSource(nameof(SimpleStartingPhrases))] string comment)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// " + comment + @"
    /// </returns>
    public bool DoSomething(object o) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_returns_tag_inside_summary_tag_and_wrong_commented_method_with_starting_phrase_([ValueSource(nameof(SimpleStartingPhrases))] string comment)
            => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// <returns>
    /// " + comment + @"
    /// </returns>
    /// </summary>
    public bool DoSomething(object o) => null;
}
");

        [TestCase("", "TODO")]
        [TestCase(" Something . ", "something")]
        [TestCase("Something.", "something")]
        [TestCase("If the stuff is done, True; False else.", "the stuff is done")]
        [TestCase("When the stuff is done, True; False else.", "the stuff is done")]
        [TestCase("In case the stuff is done, True; False else.", "the stuff is done")]
        [TestCase("In case that the stuff is done, True; else False.", "the stuff is done")]
        [TestCase("If <see langword=\"true\"/> calling method should return, otherwise not", "calling method should return")]
        [TestCase("<see langword=\"true\"/> in case that the stuff is done, in all other cases returns <see langword=\"false\"/>.", "the stuff is done")]
        public void Code_gets_fixed_for_non_generic_method_(string comment, string fixedPhrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + comment + @"</returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if " + fixedPhrase + @"; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("", "TODO")]
        [TestCase(" Something . ", "something")]
        [TestCase("Something.", "something")]
        [TestCase("If the stuff is done, True; False else.", "the stuff is done")]
        public void Code_gets_fixed_for_generic_method_(string comment, string fixedPhrase)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + comment + @"</returns>
    public Task<bool> DoSomething(object o) => throw new NotSupportedException();
}
";

            var fixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// A task that will complete with a result of <see langword=""true""/> if " + fixedPhrase + @", otherwise with a result of <see langword=""false""/>.
    /// </returns>
    public Task<bool> DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase(@"<see langword=""true""/> if something; <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langword=""true""/> if something, <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langword=""true""/> if something; otherwise <see langword=""false""/>.")]
        [TestCase(@"<see langword=""true""/> if something, otherwise <see langword=""false""/>.")]
        [TestCase(@"<see langword=""true""/> if something, otherwise returns <see langword=""false""/>.")]
        [TestCase(@"<see langword=""true""/> if something, returns otherwise <see langword=""false""/>.")]
        [TestCase(@"<see langword=""true""/>, if something; <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langword=""true""/>; if something; <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langword=""true""/>: if something; <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langword=""true""/> - if something; <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langword=""true""/> - If something; <see langword=""false""/> otherwise.")]
        [TestCase(@"<see langword=""true""/> something.")]
        [TestCase(@"<see langword=""true""/> - something.")]
        [TestCase("true if something. Otherwise false.")]
        [TestCase("<b>true</b> if something. Otherwise <b>false</b>.")]
        [TestCase("<c>true</c> if something. Otherwise <c>false</c>.")]
        [TestCase("<c>true</c> if something; otherwise, <c>false</c>.")]
        [TestCase("<code>true</code> if something; otherwise, <code>false</code>.")]
        [TestCase("True if something. False otherwise.")]
        [TestCase("True if something, otherwise False.")]
        [TestCase("True if something, False otherwise.")]
        [TestCase("true if something, false otherwise.")]
        [TestCase("True if something, otherwise returns False.")]
        [TestCase("TRUE if something, otherwise returns FALSE.")]
        [TestCase("TRUE: if something, otherwise returns FALSE.")]
        [TestCase("TRUE, if something, otherwise returns FALSE.")]
        [TestCase("Returns True if something, otherwise returns False.")]
        [TestCase("Returns True if something, returns otherwise False.")]
        [TestCase(@"Returns <see langword=""true""/> if something.")]
        [TestCase(@"Returns <see langref=""true""/> if something.")]
        [TestCase(@"Returns <see langref=""true""/> if something, otherwise returns <see langref=""false""/>.")]
        [TestCase("true: if something, false: otherwise.")]
        [TestCase("true: if something,\r\n/// false: otherwise.")]
        [TestCase("true: if something, else it returns false.")]
        [TestCase("true, if something, else it returns false.")]
        [TestCase("true if something, else with false.")]
        [TestCase("true, if something, false else.")]
        public void Code_gets_fixed_for_almost_correct_comment_on_non_generic_method_(string comment)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + comment + @"</returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if something; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase(@"<see langword=""false""/> in any other case.")]
        [TestCase(@"<see langword=""false""/> in any other cases.")]
        [TestCase(@"<see langword=""false""/> in any of the other cases.")]
        [TestCase(@"<see langword=""false""/> in all other case.")]
        [TestCase(@"<see langword=""false""/> in all other cases.")]
        [TestCase(@"<see langword=""false""/> in all of the other cases.")]
        [TestCase(@"<see langword=""false""/> in each other case.")]
        [TestCase(@"<see langword=""false""/> in each other cases.")]
        [TestCase(@"<see langword=""false""/> in each of the other cases.")]
        [TestCase(@"<see langword=""false""/> in the other cases.")]
        [TestCase(@"<see langword=""false""/> in the other case.")]
        [TestCase(@"<see langword=""false""/> in other cases.")]
        [TestCase(@"<see langword=""false""/> in other case.")]
        [TestCase("false in any other case.")]
        [TestCase("false in any other cases.")]
        [TestCase("false in any of the other cases.")]
        [TestCase("false in all other case.")]
        [TestCase("false in all other cases.")]
        [TestCase("false in all of the other cases.")]
        [TestCase("false in each other case.")]
        [TestCase("false in each other cases.")]
        [TestCase("false in each of the other cases.")]
        [TestCase("false in the other cases.")]
        [TestCase("false in the other case.")]
        [TestCase("false in other cases.")]
        [TestCase("false in other case.")]
        public void Code_gets_fixed_for_almost_correct_comment_with_trailing_other_cases_(string comment)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns><see langword=""true""/> if something, " + comment + @"</returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if something; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiline_comment_on_non_generic_method_variant_1()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>True if <paramref name=""o""/> contains something different than <paramref name=""o""/>,
    /// false otherwise.</returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if <paramref name=""o""/> contains something different than <paramref name=""o""/>; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiline_comment_on_non_generic_method_variant_2()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>will return <see langword=""true""/> if subset is found in <paramref name=""superset""/> or if subset
    /// is empty; <see langword=""false""/> otherwise. </returns>
    public bool DoSomething(object superset) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if subset is found in <paramref name=""superset""/> or if subset
    /// is empty; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object superset) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiline_comment_on_non_generic_method_variant_3()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// If the specified window true
    /// XXX,
    /// YYY,
    /// ZZZ.
    /// </returns>
    public bool DoSomething(object superset) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if the specified window 
    /// XXX,
    /// YYY,
    /// ZZZ; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object superset) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_references_in_comment_on_non_generic_method()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns><see langword=""true""/> if the content of <paramref name=""o""/> matches <paramref name=""o""/>; <see langword=""false""/> otherwise.</returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if the content of <paramref name=""o""/> matches <paramref name=""o""/>; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("A task that represents the asynchronous operation. The Result indicates whether something.")]
        [TestCase("A task that represents the operation. The Result indicates whether something.")]
        [TestCase("A task representing the asynchronous operation. The Result indicates whether something.")]
        [TestCase("A task representing the operation. The Result indicates whether something.")]
        [TestCase("An task that represents the asynchronous operation. The Result indicates whether something.")]
        [TestCase("An task that represents the operation. The Result indicates whether something.")]
        [TestCase(@"A task that completes with a result of <see langword=""true""/> if something, <see langword=""false""/> otherwise.")]
        [TestCase(@"A task that completes with a result of <see langword=""true""/> if something, otherwise, <see langword=""false""/>.")]
        [TestCase(@"A task that completes with a result of <see langword=""true""/> if something; <see langword=""false""/> otherwise.")]
        [TestCase(@"A task that completes with a result of <see langword=""true""/> if something; otherwise, <see langword=""false""/>.")]
        [TestCase(@"A task that has the result <see langword=""true""/> if something, otherwise the task has the result <see langword=""false""/>.")]
        [TestCase(@"A task that will complete with a result of <see langword=""true""/> if something, <see langword=""false""/> otherwise.")]
        [TestCase(@"A task that will complete with a result of <see langword=""true""/> if something, otherwise, <see langword=""false""/>.")]
        [TestCase(@"A task that will complete with a result of <see langword=""true""/> if something; <see langword=""false""/> otherwise.")]
        [TestCase(@"A task that will complete with a result of <see langword=""true""/> if something; otherwise, <see langword=""false""/>.")]
        [TestCase(@"Returns a task that completes with a result of <see langword=""true""/> if something, <see langword=""false""/> otherwise.")]
        [TestCase(@"Returns a task that completes with a result of <see langword=""true""/> if something, returns <see langword=""false""/> otherwise.")]
        [TestCase(@"Returns a task that will complete with a result of <see langword=""true""/> if something, <see langword=""false""/> otherwise.")]
        [TestCase(@"Returns a task that will complete with a result of <see langword=""true""/> if something, returns <see langword=""false""/> otherwise.")]
        public void Code_gets_fixed_for_almost_correct_comment_on_generic_method_(string comment)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + comment + @"</returns>
    public Task<bool> DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// A task that will complete with a result of <see langword=""true""/> if something, otherwise with a result of <see langword=""false""/>.
    /// </returns>
    public Task<bool> DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase(@"<see langword=""false""/> if something; <see langword=""true""/> otherwise.")]
        [TestCase(@"<see langword=""false""/> if something, <see langword=""true""/> otherwise.")]
        [TestCase(@"<see langword=""false""/> if something; otherwise <see langword=""true""/>.")]
        [TestCase(@"<see langword=""false""/> if something, otherwise <see langword=""true""/>.")]
        [TestCase("false if something. Otherwise true.")]
        [TestCase("<b>false</b> if something. Otherwise <b>true</b>.")]
        [TestCase("<c>false</c> if something. Otherwise <c>true</c>.")]
        [TestCase("<value>false</value> if something. Otherwise <value>true</value>.")]
        [TestCase("False if something, otherwise True.")]
        [TestCase("False if something, True otherwise.")]
        [TestCase("false if something, true otherwise.")]
        [TestCase("False if something, otherwise returns True.")]
        [TestCase("FALSE if something, otherwise returns TRUE.")]
        [TestCase("FALSE: if something, otherwise returns TRUE.")]
        [TestCase("Returns False if something, otherwise returns True.")]
        [TestCase("Returns False if something, returns otherwise True.")]
        [TestCase(@"Returns <see langref=""false""/> if something.")]
        [TestCase(@"Returns <see langword=""false""/> if something.")]
        [TestCase("false: if something, true: otherwise.")]
        public void Code_gets_not_fixed_for_almost_correct_comment_on_non_generic_method_if_starting_with_false_(string comment)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + comment + @"</returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, originalCode);
        }

        [TestCase(@"A task that will complete with a result of <see langword=""false""/> if something; otherwise, <see langword=""true""/>.")]
        [TestCase(@"A task that will complete with a result of <see langword=""false""/> if something, otherwise, <see langword=""true""/>.")]
        [TestCase(@"A task that will complete with a result of <see langword=""false""/> if something; <see langword=""true""/> otherwise.")]
        [TestCase(@"A task that will complete with a result of <see langword=""false""/> if something, <see langword=""true""/> otherwise.")]
        [TestCase(@"Returns a task that will complete with a result of <see langword=""false""/> if something, <see langword=""true""/> otherwise.")]
        [TestCase(@"Returns a task that will complete with a result of <see langword=""false""/> if something, returns <see langword=""true""/> otherwise.")]
        public void Code_gets_not_fixed_for_almost_correct_comment_on_generic_method_if_starting_with_false_(string comment)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + comment + @"</returns>
    public Task<bool> DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, originalCode);
        }

        [Test]
        public void Code_fix_keeps_spaces_before_paramref_tag()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if something in the given <paramref name=""o""/>
    /// is there; <see langword=""false""/> otherwise
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if something in the given <paramref name=""o""/>
    /// is there; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_fix_places_fix_after_starting_para_tag()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <para>
    /// if something is there; <see langword=""false""/> otherwise
    /// </para>
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <para>
    /// <see langword=""true""/> if something is there; otherwise, <see langword=""false""/>.
    /// </para>
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_simple_starting_phrase_([ValueSource(nameof(SimpleStartingPhrases))] string comment)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + comment + @"something.</returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if something; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_returns_tag_inside_summary_tag_([ValueSource(nameof(SimpleStartingPhrases))] string comment)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// <returns>
    /// " + comment + @"something.
    /// </returns>
    /// </summary>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// <returns>
    /// <see langword=""true""/> if something; otherwise, <see langword=""false""/>.
    /// </returns>
    /// </summary>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("if ")]
        [TestCase("If ")]
        [TestCase("whether ")]
        [TestCase("Whether ")]
        [TestCase("True when")]
        [TestCase("True means")]
        [TestCase("True means that")]
        [TestCase("True of")] // typo
        public void Code_gets_fixed_for_specific_starting_phrase_(string comment)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + comment + @" something.</returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if something; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_fix_recognizes_Otherwise_at_end_of_first_line_in_multi_line_comment_([Values("Otherwise ", "Otherwise")] string lastWordOnFirstLine)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if something is there. " + lastWordOnFirstLine + @"
    /// <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// <see langword=""true""/> if something is there; otherwise, <see langword=""false""/>.
    /// </returns>
    public bool DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2032_CodeFixProvider();
    }
}