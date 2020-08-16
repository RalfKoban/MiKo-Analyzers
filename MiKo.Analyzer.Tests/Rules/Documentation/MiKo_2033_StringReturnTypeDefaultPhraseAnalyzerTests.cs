using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NCrunch.Framework;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture, Isolated]
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
        public void Code_gets_fCode_gets_fixed_for_generic_methodixed()
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
    /// <returns>A task that represents the asynchronous operation. The <see cref=""Task{TResult}.Result""/> property on the task object returns a <see cref=""string""/> that contains Something.</returns>
    public Task<string> DoSomething(object o) => null;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2033_CodeFixProvider();
    }
}