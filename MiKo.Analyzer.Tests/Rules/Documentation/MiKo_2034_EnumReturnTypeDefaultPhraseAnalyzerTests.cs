using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] EnumOnlyReturnValues =
                                                                [
                                                                    "StringComparison",
                                                                    "System.StringComparison",
                                                                ];

        private static readonly string[] EnumTaskReturnValues =
                                                                [
                                                                    "Task<StringComparison>",
                                                                    "Task<System.StringComparison>",
                                                                    "System.Threading.Tasks.Task<StringComparison>",
                                                                    "System.Threading.Tasks.Task<System.StringComparison>",
                                                                ];

        private static readonly string[] EnumReturnValues = [.. EnumOnlyReturnValues, .. EnumTaskReturnValues];

        [Test]
        public void No_issue_is_reported_for_undocumented_method_([ValueSource(nameof(EnumReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_property_([ValueSource(nameof(EnumReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_returning_non_enum_type_(
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
        public void No_issue_is_reported_for_non_generic_enum_return_with_standard_phrase_(
                                                                                       [Values("returns", "value")] string xmlTag,
                                                                                       [ValueSource(nameof(EnumOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// The enumerated constant that is the whatever value.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_Task_enum_return_with_standard_phrase_(
                                                                                [Values("returns", "value")] string xmlTag,
                                                                                [Values("", " ")] string space,
                                                                                [ValueSource(nameof(EnumTaskReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A task that represents the asynchronous operation. The value of the <see cref=""System.Threading.Tasks.Task{TResult}.Result" + space + @"/> parameter contains the enumerated constant that is the value.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_enum_return_with_non_standard_phrase_(
                                                                               [Values("returns", "value")] string xmlTag,
                                                                               [Values("A whatever", "An whatever", "The whatever")] string comment,
                                                                               [ValueSource(nameof(EnumReturnValues))] string returnType)
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

        [TestCase("Something.", "the something.")]
        [TestCase("A something.", "the something.")]
        [TestCase("a something.", "the something.")]
        [TestCase("An something.", "the something.")]
        [TestCase("an something.", "the something.")]
        [TestCase("The something.", "the something.")]
        [TestCase("the something.", "the something.")]
        [TestCase("""Something, such as <see cref="string.Empty"/>.""", """the something, such as <see cref="string.Empty"/>.""")]
        [TestCase("""If something, the result is <see cref="StringComparison.Ordinal"/>.""", """<see cref="StringComparison.Ordinal"/> if something.""")]
        [TestCase("""If something, the result will be <see cref="StringComparison.Ordinal"/>.""", """<see cref="StringComparison.Ordinal"/> if something.""")]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_enum_(string originalText, string fixedText)
        {
            var originalCode = @"
using System;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + originalText + @"</returns>
    public StringComparison DoSomething(object o) => null;
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
    /// The enumerated constant that is " + fixedText + @"
    /// </returns>
    public StringComparison DoSomething(object o) => null;
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Something.", "the something.")]
        [TestCase("A something.", "the something.")]
        [TestCase("a something.", "the something.")]
        [TestCase("An something.", "the something.")]
        [TestCase("an something.", "the something.")]
        [TestCase("The something.", "the something.")]
        [TestCase("the something.", "the something.")]
        [TestCase("""Something, such as <see cref="string.Empty"/>.""", """the something, such as <see cref="string.Empty"/>.""")]
        [TestCase("""If something, the result is <see cref="StringComparison.Ordinal"/>.""", """<see cref="StringComparison.Ordinal"/> if something.""")]
        [TestCase("""If something, the result will be <see cref="StringComparison.Ordinal"/>.""", """<see cref="StringComparison.Ordinal"/> if something.""")]
        [TestCase("""If something, the result is <see cref="StringComparison.Ordinal"/>""", """<see cref="StringComparison.Ordinal"/> if something""")]
        [TestCase("""If something, the result will be <see cref="StringComparison.Ordinal"/>""", """<see cref="StringComparison.Ordinal"/> if something""")]
        public void Code_gets_fixed_by_replacing_with_standard_phrase_for_Task_enum_(string originalText, string fixedText)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>" + originalText + @"</returns>
    public Task<StringComparison> DoSomething(object o) => null;
}
";

            var fixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains the enumerated constant that is " + fixedText + @"
    /// </returns>
    public Task<StringComparison> DoSomething(object o) => null;
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2034_CodeFixProvider();
    }
}