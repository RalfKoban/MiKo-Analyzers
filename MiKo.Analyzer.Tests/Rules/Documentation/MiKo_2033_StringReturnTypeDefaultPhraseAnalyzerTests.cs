using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2033_StringReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] StringOnlyReturnValues =
                                                                  {
                                                                      "string",
                                                                      "String",
                                                                      "System.String",
                                                                  };

        private static readonly string[] StringTaskReturnValues =
                                                                  {
                                                                      "Task<string>",
                                                                      "Task<String>",
                                                                      "Task<System.String>",
                                                                      "System.Threading.Tasks.Task<string>",
                                                                      "System.Threading.Tasks.Task<String>",
                                                                      "System.Threading.Tasks.Task<System.String>",
                                                                  };

        private static readonly string[] StringReturnValues = StringOnlyReturnValues.Concat(StringTaskReturnValues).ToArray();

        [Test]
        public void No_issue_is_reported_for_uncommented_method_([ValueSource(nameof(StringReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property_([ValueSource(nameof(StringReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a_(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("void", "int", "Task", "Task<int>", "Task<bool>")] string returnType)
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
        public void No_issue_is_reported_for_correctly_commented_String_only_method_(
                                                                                 [Values("returns", "value")] string xmlTag,
                                                                                 [Values("", " ")] string space,
                                                                                 [ValueSource(nameof(StringOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A " + "<see cref=\"" + returnType + "\"" + space + @"/> that contains something.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_consist_String_only_method_(
                                                                                         [Values("returns", "value")] string xmlTag,
                                                                                         [Values("", " ")] string space,
                                                                                         [ValueSource(nameof(StringOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A " + "<see cref=\"" + returnType + "\"" + space + @"/> that consists of something.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_ToString_method_(
                                                                              [Values("returns")] string xmlTag,
                                                                              [Values("", " ")] string space,
                                                                              [ValueSource(nameof(StringOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A " + "<see cref=\"" + returnType + "\"" + space + @"/> that represents the current object.
    /// </" + xmlTag + @">
    public " + returnType + @" ToString() => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_String_Task_method_(
                                                                                 [Values("returns", "value")] string xmlTag,
                                                                                 [Values("", " ")] string space,
                                                                                 [ValueSource(nameof(StringTaskReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A task that represents the asynchronous operation. The <see cref=""System.Threading.Tasks.Task{TResult}.Result" + "\"" + space + @" /> property on the task object returns a <see cref=""System.String" + "\"" + space + @"/> that contains something.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_String_interned_method() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// An interned copy of the <see cref=""string""/> that contains something.
    /// </returns>
    public string DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_method_(
                                                                 [Values("returns", "value")] string xmlTag,
                                                                 [Values("A whatever", "An whatever", "The whatever")] string comment,
                                                                 [ValueSource(nameof(StringReturnValues))] string returnType)
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
        public void Code_gets_fixed_for_non_generic_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>Something.</returns>
    public string DoSomething(object o) => null;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that contains something.
    /// </returns>
    public string DoSomething(object o) => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_generic_method_with_line_break()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// Something.
    /// </returns>
    public string DoSomething(object o) => null;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that contains something.
    /// </returns>
    public string DoSomething(object o) => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("Return")]
        [TestCase("Returns")]
        [TestCase("return")]
        [TestCase("returns")]
        [TestCase("Contains")]
        [TestCase("Contain")]
        [TestCase("A string with")]
        [TestCase("A string that represents")]
        [TestCase("A string which represents")]
        [TestCase("A string that contains")]
        [TestCase("A string which contains")]
        [TestCase("A string containing")]
        [TestCase("A string representing")]
        [TestCase("String with")]
        [TestCase("String that represents")]
        [TestCase("String which represents")]
        [TestCase("String that contains")]
        [TestCase("String which contains")]
        [TestCase("String representing")]
        [TestCase("String containing")]
        [TestCase("Returns a string with")]
        public void Code_gets_fixed_for_non_generic_method_starting_with_(string phrase)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + phrase + @" the text.</returns>
    public string DoSomething(object o) => null;
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that contains the text.
    /// </returns>
    public string DoSomething(object o) => null;
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_generic_method()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>Something.</returns>
    public Task<string> DoSomething(object o) => null;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The <see cref=""Task{TResult}.Result""/> property on the task object returns a <see cref=""string""/> that contains something.
    /// </returns>
    public Task<string> DoSomething(object o) => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test(Description = "Bugfix for https://github.com/RalfKoban/MiKo-Analyzers/issues/366")]
        public void Code_gets_fixed_for_issue_366()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Gets the literal ""Foo"".
    /// </summary>
    /// <value>
    /// The Foo.
    /// </value>
    public string Foo => ""Foo"";

    /// <summary>
    /// Gets the literal ""Bar"".
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that contains the Bar.
    /// </returns>
    public string Bar() => ""Bar"";
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Gets the literal ""Foo"".
    /// </summary>
    /// <value>
    /// A <see cref=""string""/> that contains the Foo.
    /// </value>
    public string Foo => ""Foo"";

    /// <summary>
    /// Gets the literal ""Bar"".
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that contains the Bar.
    /// </returns>
    public string Bar() => ""Bar"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test(Description = "Reverse-ordered Bugfix for https://github.com/RalfKoban/MiKo-Analyzers/issues/366")]
        public void Code_gets_fixed_for_issue_366_order_reversed()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Gets the literal ""Bar"".
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that contains the Bar.
    /// </returns>
    public string Bar() => ""Bar"";

    /// <summary>
    /// Gets the literal ""Foo"".
    /// </summary>
    /// <value>
    /// The Foo.
    /// </value>
    public string Foo => ""Foo"";
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Gets the literal ""Bar"".
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that contains the Bar.
    /// </returns>
    public string Bar() => ""Bar"";

    /// <summary>
    /// Gets the literal ""Foo"".
    /// </summary>
    /// <value>
    /// A <see cref=""string""/> that contains the Foo.
    /// </value>
    public string Foo => ""Foo"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("", Description = "Bugfix for https://github.com/RalfKoban/MiKo-Analyzers/issues/401")]
        [TestCase("some ", Description = "Bugfix for https://github.com/RalfKoban/MiKo-Analyzers/issues/401")]
        public void Code_gets_fixed_for_non_generic_method_and_issue_401_when_phrase_is_almost_correct_(string phrase)
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// A <see cref=""string""/> returning ###<c>Foo</c>.
    /// <see cref=""TestMe"" /> for more details.
    /// </value>
    public string Foo => ""Foo"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// A <see cref=""string""/> that contains ###<c>Foo</c>.
    /// <see cref=""TestMe"" /> for more details.
    /// </value>
    public string Foo => ""Foo"";
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", phrase), FixedCode.Replace("###", phrase));
        }

        [Test(Description = "Bugfix for https://github.com/RalfKoban/MiKo-Analyzers/issues/401")]
        public void Code_gets_fixed_for_non_generic_method_and_issue_401_when_complete_phrase_gets_added()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// The <c>Foo</c>.
    /// <see cref=""TestMe"" /> for more details.
    /// </value>
    public string Foo => ""Foo"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// A <see cref=""string""/> that contains the <c>Foo</c>.
    /// <see cref=""TestMe"" /> for more details.
    /// </value>
    public string Foo => ""Foo"";
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_generic_method_and_issue_401_when_phrase_is_almost_correct_variant_1()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The <see cref=""Task{TResult}.Result""/> property on the task object returns a <see cref=""string""/> containing the <c>Foo</c>.</returns>
    public Task<string> DoSomething(object o) => null;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The <see cref=""Task{TResult}.Result""/> property on the task object returns a <see cref=""string""/> that contains the <c>Foo</c>.
    /// </returns>
    public Task<string> DoSomething(object o) => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_generic_method_and_issue_401_when_phrase_is_almost_correct_variant_2()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The <see cref=""Task{TResult}.Result""/> property on the task object returns a <see cref=""string""/> containing the <c>Foo</c>.
    /// </returns>
    public Task<string> DoSomething(object o) => null;
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The <see cref=""Task{TResult}.Result""/> property on the task object returns a <see cref=""string""/> that contains the <c>Foo</c>.
    /// </returns>
    public Task<string> DoSomething(object o) => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_generic_method_and_almost_correct_phrase_([Values("that returns", "which returns", "returning", "which contains")] string text)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// A <see cref=""string""/> " + text + @" something.
    /// </value>
    public string Something => ""Something"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// A <see cref=""string""/> that contains something.
    /// </value>
    public string Something => ""Something"";
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_generic_method_and_almost_correct_phrase_without_see_Cref_([Values("that returns", "which returns", "returning", "which contains", "that contains", "containing")] string text)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// A string " + text + @" something.
    /// </value>
    public string Something => ""Something"";
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// A <see cref=""string""/> that contains something.
    /// </value>
    public string Something => ""Something"";
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_overridden_ToString_method()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => base.ToString();
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that represents the current object.
    /// </returns>
    public override string ToString() => base.ToString();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2033_CodeFixProvider();
    }
}