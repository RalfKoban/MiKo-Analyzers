using System.Linq;

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
                                                                {
                                                                    "StringComparison",
                                                                    "System.StringComparison",
                                                                };

        private static readonly string[] EnumTaskReturnValues =
                                                                {
                                                                    "Task<StringComparison>",
                                                                    "Task<System.StringComparison>",
                                                                    "System.Threading.Tasks.Task<StringComparison>",
                                                                    "System.Threading.Tasks.Task<System.StringComparison>",
                                                                };

        private static readonly string[] EnumReturnValues = EnumOnlyReturnValues.Concat(EnumTaskReturnValues).ToArray();

        [Test]
        public void No_issue_is_reported_for_uncommented_method_([ValueSource(nameof(EnumReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property_([ValueSource(nameof(EnumReturnValues))] string returnType) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Enum_only_method_(
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
        public void No_issue_is_reported_for_correctly_commented_Enum_Task_method_(
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
        public void An_issue_is_reported_for_wrong_commented_method_(
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

        [TestCase("Something.", "something.")]
        [TestCase(@"Something, such as <see cref=""string.Empty""/>.", @"something, such as <see cref=""string.Empty""/>.")]
        [TestCase("The something.", "something.")]
        [TestCase("the something.", "something.")]
        [TestCase("A something.", "something.")]
        [TestCase("a something.", "something.")]
        [TestCase("An something.", "something.")]
        [TestCase("an something.", "something.")]
        public void Code_gets_fixed_for_non_generic_method_(string originalText, string fixedText)
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
    /// The enumerated constant that is the " + fixedText + @"
    /// </returns>
    public StringComparison DoSomething(object o) => null;
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCase("Something.", "something.")]
        [TestCase(@"Something, such as <see cref=""string.Empty""/>.", @"something, such as <see cref=""string.Empty""/>.")]
        [TestCase("The something.", "something.")]
        [TestCase("the something.", "something.")]
        public void Code_gets_fixed_for_generic_method_(string originalText, string fixedText)
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
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains the enumerated constant that is the " + fixedText + @"
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