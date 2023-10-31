using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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

        [TestCase("ContinueWith", "A new continuation task.")]
        [TestCase("FromCanceled", "The canceled task.")]
        [TestCase("FromException", "The faulted task.")]
        [TestCase("FromResult", "The successfully completed task.")]
        [TestCase("Run", "A task that represents the work queued to execute in the thread pool.")]
        [TestCase("WhenAll", "A task that represents the completion of all of the supplied tasks.")]
        [TestCase("WhenAny", "A task that represents the completion of one of the supplied tasks. Its <see cref=\"Task{TResult}.Result\" /> is the task that completed first.")]
        [TestCase("WhenAny", "A task that represents the completion of one of the supplied tasks. Its <see cref=\"Task{TResult}.Result\"/> is the task that completed first.")]
        [TestCase("WhenAny", "A task that represents the completion of one of the supplied tasks. Its <see cref=\"System.Threading.Tasks.Task{TResult}.Result\" /> is the task that completed first.")]
        [TestCase("WhenAny", "A task that represents the completion of one of the supplied tasks. Its <see cref=\"System.Threading.Tasks.Task{TResult}.Result\"/> is the task that completed first.")]
        [TestCase("WhenAny", "A <see cref=\"System.Threading.Tasks.Task{TResult}\" /> that represents the completion of one of the supplied tasks. Its <see cref=\"System.Threading.Tasks.Task{TResult}.Result\" /> is the task that completed first.")]
        [TestCase("WhenAny", "A <see cref=\"System.Threading.Tasks.Task{TResult}\" /> that represents the completion of one of the supplied tasks. Its <see cref=\"System.Threading.Tasks.Task{TResult}.Result\"/> is the task that completed first.")]
        [TestCase("WhenAny", "A <see cref=\"System.Threading.Tasks.Task{TResult}\"/> that represents the completion of one of the supplied tasks. Its <see cref=\"System.Threading.Tasks.Task{TResult}.Result\" /> is the task that completed first.")]
        [TestCase("WhenAny", "A <see cref=\"System.Threading.Tasks.Task{TResult}\"/> that represents the completion of one of the supplied tasks. Its <see cref=\"System.Threading.Tasks.Task{TResult}.Result\"/> is the task that completed first.")]
        public void No_issue_is_reported_for_special_method_(string methodName, string comment)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// " + comment + @"
    /// </returns>
    public Task " + methodName + @"() => throw new NotSupportedException();
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
        [TestCase("Task, that executes the operation.")]
        [TestCase("The task of the operation.")]
        [TestCase("An awaitable task.")]
        [TestCase("An awaitable task")]
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
        [TestCase(@"A <see cref=""Task""/> that contains something.", "something.")]
        [TestCase(@"A <see cref=""Task""/> that represents the asynchronous operation.", "")]
        [TestCase(@"A <see cref=""Task""/> which contains something.", "something.")]
        [TestCase(@"A <see cref=""Task{TResult}""/> that contains something.", "something.")]
        [TestCase(@"A <see cref=""Task{TResult}""/> that represents the asynchronous operation.", "")]
        [TestCase(@"A <see cref=""Task{TResult}""/> which contains something.", "something.")]
        [TestCase("A result containing something.", "something.")]
        [TestCase(@"A result describing something, such as <see cref=""string.Empty""/>.", @"a result describing something, such as <see cref=""string.Empty""/>.")]
        [TestCase("A result that contains something.", "something.")]
        [TestCase("A result which contains something.", "something.")]
        [TestCase("A task that contains something.", "something.")]
        [TestCase("A task that represents the asynchronous operation.", "")]
        [TestCase("A task which contains something.", "something.")]
        [TestCase(@"The <see cref=""Task""/> that contains something.", "something.")]
        [TestCase(@"The <see cref=""Task""/> which contains something.", "something.")]
        [TestCase(@"The <see cref=""Task{TResult}""/> that contains something.", "something.")]
        [TestCase(@"The <see cref=""Task{TResult}""/> which contains something.", "something.")]
        [TestCase("The task that contains something.", "something.")]
        [TestCase("The task which contains something.", "something.")]
        [TestCase("The result of the whole stuff.", "the result of the whole stuff.")]
        [TestCase(@"A <see cref=""Task""/> containing the <see cref=""int""/> of this single operation.", @"the <see cref=""int""/> of this single operation.")]
        [TestCase(@"<see langword=""true""/> if something, <see langword=""false""/> in all other cases.", @"<see langword=""true""/> if something, <see langword=""false""/> in all other cases.")]
        [TestCase(@"A task that can be used to await and the new <see cref=""string"" />.", @"the new <see cref=""string"" />.")]
        [TestCase(@"A task that can be used to await. The new <see cref=""string"" />.", @"the new <see cref=""string"" />.")]
        [TestCase("A task that can be used to await.", "")]
        [TestCase("A task that can be used to await", "")]
        [TestCase(@"A task to await and the new <see cref=""string"" />.", @"the new <see cref=""string"" />.")]
        [TestCase(@"A task to await. The new <see cref=""string"" />.", @"the new <see cref=""string"" />.")]
        [TestCase("A task to await.", "")]
        [TestCase("A task to await", "")]
        [TestCase(@"An awaitable task and the new <see cref=""string"" />.", @"the new <see cref=""string"" />.")]
        [TestCase(@"An awaitable task. The new <see cref=""string"" />.", @"the new <see cref=""string"" />.")]
        [TestCase("An awaitable task.", "")]
        [TestCase("An awaitable task", "")]
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

        [TestCase("Task ContinueWith()", "A new continuation task.")]
        [TestCase("Task FromCanceled()", "The canceled task.")]
        [TestCase("Task FromException()", "The faulted task.")]
        [TestCase("Task FromResult()", "The successfully completed task.")]
        [TestCase("Task Run()", "A task that represents the work queued to execute in the thread pool.")]
        [TestCase("Task WhenAll()", "A task that represents the completion of all of the supplied tasks.")]
        [TestCase("Task WhenAny()", "A task that represents the completion of one of the supplied tasks. Its <see cref=\"Task{TResult}.Result\"/> is the task that completed first.")]
        [TestCase("Task<int> WhenAny()", "A task that represents the completion of one of the supplied tasks. Its <see cref=\"Task{TResult}.Result\"/> is the task that completed first.")]
        public void Code_gets_fixed_for_special_method_(string specialMethod, string comment)
        {
            var originalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <returns>
    /// Some comment.
    /// </returns>
    public " + specialMethod + @" => throw new NotSupportedException();
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
    /// " + comment + @"
    /// </returns>
    public " + specialMethod + @" => throw new NotSupportedException();
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2031_CodeFixProvider();
    }
}