using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public Task DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property() => No_issue_is_reported_for(@"
public class TestMe
{
    public Task DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething(object o) { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a_(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("Task<bool>", "Task<string>")] string returnType)
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
        public void No_issue_is_reported_for_correctly_commented_Task_only_method_(
                                                                                [Values("returns", "value")] string xmlTag,
                                                                                [Values("task", "<see cref=\"Task\" />", "<see cref=\"Task\"/>")] string comment)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A " + comment + @" that represents the asynchronous operation.
    /// </" + xmlTag + @">
    public Task DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_Task_only_method_([Values("returns", "value")] string xmlTag, [Values("A whatever", "An whatever", "The whatever")] string comment) => An_issue_is_reported_for(@"
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
    public Task DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_generic_Task_method_(
            [Values("returns", "value")] string xmlTag,
            [Values("task", "<see cref=\"Task{TResult}\" />", "<see cref=\"Task{TResult}\"/>", "<see cref=\"System.Threading.Tasks.Task{TResult}\" />", "<see cref=\"System.Threading.Tasks.Task{TResult}\"/>")] string task,
            [Values("<see cref=\"System.Threading.Tasks.Task{TResult}.Result\" />", "<see cref=\"System.Threading.Tasks.Task{TResult}.Result\"/>")] string result)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A " + task + " that represents the asynchronous operation. The value of the " + result + @" parameter contains something.
    /// </" + xmlTag + @">
    public Task<int> DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_generic_Task_method_([Values("returns", "value")] string xmlTag, [Values("A whatever", "An whatever", "The whatever")] string comment) => An_issue_is_reported_for(@"
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
    public Task<int> DoSomething(object o) => null;
}
");

        [TestCase("Something.")]
        [TestCase("A result describing something.")]
        [TestCase(@"A result describing something, such as <see cref=""string.Empty""/>.")]
        public void Code_gets_fixed_for_non_generic_method_(string originalText)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + originalText + @"</returns>
    public Task DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public Task DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("", "")]
        [TestCase("Something.", "something.")]
        [TestCase("A result describing something.", "a result describing something.")]
        [TestCase(@"A <see cref=""Task""/> that contains something.", @"something.")]
        [TestCase(@"A <see cref=""Task""/> that represents the asynchronous operation.", @"")]
        [TestCase(@"A <see cref=""Task""/> which contains something.", @"something.")]
        [TestCase(@"A <see cref=""Task{TResult}""/> that contains something.", @"something.")]
        [TestCase(@"A <see cref=""Task{TResult}""/> that represents the asynchronous operation.", @"")]
        [TestCase(@"A <see cref=""Task{TResult}""/> which contains something.", @"something.")]
        [TestCase(@"A result containing something.", @"something.")]
        [TestCase(@"A result describing something, such as <see cref=""string.Empty""/>.", @"a result describing something, such as <see cref=""string.Empty""/>.")]
        [TestCase(@"A result that contains something.", @"something.")]
        [TestCase(@"A result which contains something.", @"something.")]
        [TestCase(@"A task that contains something.", @"something.")]
        [TestCase(@"A task that represents the asynchronous operation.", @"")]
        [TestCase(@"A task which contains something.", @"something.")]
        [TestCase(@"The <see cref=""Task""/> that contains something.", @"something.")]
        [TestCase(@"The <see cref=""Task""/> which contains something.", @"something.")]
        [TestCase(@"The <see cref=""Task{TResult}""/> that contains something.", @"something.")]
        [TestCase(@"The <see cref=""Task{TResult}""/> which contains something.", @"something.")]
        [TestCase(@"The task that contains something.", @"something.")]
        [TestCase(@"The task which contains something.", @"something.")]
        [TestCase(@"The result of the whole stuff.", @"the result of the whole stuff.")]
        [TestCase(@"A <see cref=""Task""/> containing the <see cref=""int""/> of this single operation.", @"the <see cref=""int""/> of this single operation.")]
        public void Code_gets_fixed_for_generic_method_(string originalText, string fixedText)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>" + originalText + @"</returns>
    public Task<int> DoSomething(object o) => throw new NotSupportedException();
}
";

            var fixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains " + fixedText + @"
    /// </returns>
    public Task<int> DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2031_CodeFixProvider();
    }
}