using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2031_TaskReturnValueDefaultPhraseAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_that_returns_a([Values("returns", "value")] string xmlTag,
                                                                   [Values("Task<bool>", "Task<string>")] string returnType) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Task_only_method(
                                                                                [Values("returns", "value")] string xmlTag,
                                                                                [Values(
                                                                                    "A task that represents the asynchronous operation.",
                                                                                    "A <see cref=\"Task\" /> that represents the asynchronous operation.",
                                                                                    "A <see cref=\"Task\"/> that represents the asynchronous operation.")] string comment)
            => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_wrong_commented_Task_only_method([Values("returns", "value")] string xmlTag, [Values("A whatever", "An whatever", "The whatever")] string comment) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_generic_Task_method(
            [Values("returns", "value")] string xmlTag,
            [Values(
                "A task that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task{TResult}.Result\" /> parameter contains something.",
                "A <see cref=\"Task{TResult}\" /> that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task{TResult}.Result\" /> parameter contains something.",
                "A <see cref=\"Task{TResult}\"/> that represents the asynchronous operation. The value of the <see cref=\"System.Threading.Tasks.Task{TResult}.Result\" /> parameter contains something.")] string comment)
            => No_issue_is_reported_for(@"
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

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_generic_Task_method([Values("returns", "value")] string xmlTag, [Values("A whatever", "An whatever", "The whatever")] string comment) => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_2031_TaskReturnValueDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2031_TaskReturnValueDefaultPhraseAnalyzer();
    }
}