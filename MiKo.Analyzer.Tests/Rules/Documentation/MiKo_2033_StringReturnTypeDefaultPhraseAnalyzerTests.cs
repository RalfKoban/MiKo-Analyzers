﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2033_StringReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] StringOnlyReturnValues =
                                                                  [
                                                                      "string",
                                                                      "String",
                                                                      "System.String",
                                                                  ];

        private static readonly string[] StringTaskReturnValues =
                                                                  [
                                                                      "Task<string>",
                                                                      "Task<String>",
                                                                      "Task<System.String>",
                                                                      "System.Threading.Tasks.Task<string>",
                                                                      "System.Threading.Tasks.Task<String>",
                                                                      "System.Threading.Tasks.Task<System.String>",
                                                                  ];

        private static readonly string[] StringReturnValues = [.. StringOnlyReturnValues, .. StringTaskReturnValues];

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
    /// A task that represents the asynchronous operation. The value of the <see cref=""System.Threading.Tasks.Task{TResult}.Result" + "\"" + space + @" /> parameter returns a <see cref=""System.String" + "\"" + space + @"/> that contains something.
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

        [TestCase("A concatenated string containing")]
        [TestCase("A concatenated string of")]
        [TestCase("A concatenated string with")]
        [TestCase("A concatenated string that contains")]
        [TestCase("A concatenated string value containing")]
        [TestCase("A concatenated string value of")]
        [TestCase("A concatenated string value with")]
        [TestCase("A concatenated string value that contains")]
        [TestCase("A concatenated string value which contains")]
        [TestCase("A concatenated string which contains")]
        [TestCase("A new string containing")]
        [TestCase("A new string representing")]
        [TestCase("A new string that contains")]
        [TestCase("A new string that represents")]
        [TestCase("A new string which contains")]
        [TestCase("A new string which represents")]
        [TestCase("A single string containing")]
        [TestCase("A single string representing")]
        [TestCase("A single string that contains")]
        [TestCase("A single string that represents")]
        [TestCase("A single string which contains")]
        [TestCase("A single string which represents")]
        [TestCase("A single string with")]
        [TestCase("A string containing")]
        [TestCase("A string representing")]
        [TestCase("A string that contains")]
        [TestCase("A string that represents")]
        [TestCase("A string which contains")]
        [TestCase("A string which represents")]
        [TestCase("A string with")]
        [TestCase("Contain")]
        [TestCase("Contains")]
        [TestCase("return")]
        [TestCase("Return")]
        [TestCase("Returns a string with")]
        [TestCase("returns")]
        [TestCase("Returns")]
        [TestCase("String containing")]
        [TestCase("String representing")]
        [TestCase("String that contains")]
        [TestCase("String that represents")]
        [TestCase("String which contains")]
        [TestCase("String which represents")]
        [TestCase("String with")]
        [TestCase("The concatenated string containing")]
        [TestCase("The concatenated string of")]
        [TestCase("The concatenated string with")]
        [TestCase("The concatenated string that contains")]
        [TestCase("The concatenated string value containing")]
        [TestCase("The concatenated string value of")]
        [TestCase("The concatenated string value with")]
        [TestCase("The concatenated string value that contains")]
        [TestCase("The concatenated string value which contains")]
        [TestCase("The concatenated string which contains")]
        [TestCase("The new string containing")]
        [TestCase("The new string representing")]
        [TestCase("The new string that contains")]
        [TestCase("The new string that represents")]
        [TestCase("The new string which contains")]
        [TestCase("The new string which represents")]
        [TestCase("The single string containing")]
        [TestCase("The single string representing")]
        [TestCase("The single string that contains")]
        [TestCase("The single string that represents")]
        [TestCase("The single string which contains")]
        [TestCase("The single string which represents")]
        [TestCase("The single string with")]
        [TestCase("The string value with")]
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

        [TestCase("A new string surrounded by whatever", "the original value surrounded by whatever")]
        [TestCase("A new string value surrounded by whatever", "the original value surrounded by whatever")]
        [TestCase("A new string value with something", "the original value with something")]
        [TestCase("A new string value without something", "the original value without something")]
        [TestCase("A new string with something", "the original value with something")]
        [TestCase("A new string without something", "the original value without something")]
        [TestCase("The new string surrounded by whatever", "the original value surrounded by whatever")]
        [TestCase("The new string value surrounded by whatever", "the original value surrounded by whatever")]
        [TestCase("The new string value with something", "the original value with something")]
        [TestCase("The new string value without something", "the original value without something")]
        [TestCase("The new string with something", "the original value with something")]
        [TestCase("The new string without something", "the original value without something")]
        public void Code_gets_fixed_for_non_generic_method_starting_and_continuing_with_(string phrase, string continuation)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + phrase + @".</returns>
    public string DoSomething(object o) => null;
}
";

            var fixedCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A <see cref=""string""/> that contains " + continuation + @".
    /// </returns>
    public string DoSomething(object o) => null;
}
";

            VerifyCSharpFix(originalCode, fixedCode);
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter returns a <see cref=""string""/> that contains something.
    /// </returns>
    public Task<string> DoSomething(object o) => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_almost_correct_generic_method()
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
    /// A task that represents the asynchronous operation. The Result is something.
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter returns a <see cref=""string""/> that contains something.
    /// </returns>
    public Task<string> DoSomething(object o) => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_almost_correct_phrase()
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
    /// A task that represents the asynchronous operation. The <see cref=""Task{TResult}.Result""/> property on the task object returns a <see cref=""string""/> that contains something.
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter returns a <see cref=""string""/> that contains something.
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
    /// <returns>A task representing the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter returns a <see cref=""string""/> containing the <c>Foo</c>.</returns>
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter returns a <see cref=""string""/> that contains the <c>Foo</c>.
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
    /// A task representing the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter returns a <see cref=""string""/> containing the <c>Foo</c>.
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter returns a <see cref=""string""/> that contains the <c>Foo</c>.
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
        public void Code_gets_fixed_for_non_generic_method_and_almost_correct_phrase_with_see_Cref_([Values("that returns", "which returns", "returning", "which contains", "that contains", "containing")] string text)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Gets something.
    /// </summary>
    /// <value>
    /// A string " + text + @" the <see cref=""int""/> and/or <see cref=""bool""/> value of the operation.
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
    /// A <see cref=""string""/> that contains the <see cref=""int""/> and/or <see cref=""bool""/> value of the operation.
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