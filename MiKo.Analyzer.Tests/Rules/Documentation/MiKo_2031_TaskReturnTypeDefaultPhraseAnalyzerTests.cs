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

        [Test]
        public void Code_gets_fixed_for_non_generic_method()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>Something.</returns>
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

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_generic_method()
        {
            const string OriginalCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>Something.</returns>
    public Task<int> DoSomething(object o) => throw new NotSupportedException();
}
";

            const string FixedCode = @"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The value of the <see cref=""Task{TResult}.Result""/> parameter contains Something.
    /// </returns>
    public Task<int> DoSomething(object o) => throw new NotSupportedException();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2031_CodeFixProvider();
    }
}