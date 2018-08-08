using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2033_StringReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method([ValueSource(nameof(StringReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property([ValueSource(nameof(StringReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a([Values("returns", "value")] string xmlTag,
                                                                   [Values("void", "int", "Task", "Task<int>", "Task<bool>")] string returnType) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_String_only_method(
            [Values("returns", "value")] string xmlTag,
            [Values("", " ")] string space,
            [ValueSource(nameof(StringOnlyReturnValues))] string returnType) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_ToString_method(
            [Values("returns")] string xmlTag,
            [Values("", " ")] string space,
            [ValueSource(nameof(StringOnlyReturnValues))] string returnType) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_String_Task_method(
            [Values("returns", "value")] string xmlTag,
            [Values("", " ")] string space,
            [ValueSource(nameof(StringTaskReturnValues))] string returnType) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_wrong_commented_method(
            [Values("returns", "value")] string xmlTag,
            [Values("A whatever", "An whatever", "The whatever")] string comment,
            [ValueSource(nameof(StringReturnValues))] string returnType) => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2033_StringReturnTypeDefaultPhraseAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> StringOnlyReturnValues() => new[] { "string", "String", "System.String", nameof(System.String), }.ToHashSet();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> StringTaskReturnValues() => new[] { "Task<string>", "Task<String>", "Task<System.String>", }.ToHashSet();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> StringReturnValues() => StringOnlyReturnValues().Concat(StringTaskReturnValues()).ToHashSet();
    }
}