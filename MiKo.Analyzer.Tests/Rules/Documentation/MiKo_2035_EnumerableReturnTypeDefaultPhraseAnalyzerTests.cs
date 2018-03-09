using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method([ValueSource(nameof(EnumerableReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property([ValueSource(nameof(EnumerableReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a([Values("returns", "value")] string xmlTag,
                                                                   [Values("void", "int", "Task", "Task<int>", "Task<bool>")] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
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
        public void No_issue_is_reported_for_correctly_commented_Enumerable_only_method(
            [Values("returns", "value")] string xmlTag,
            [ValueSource(nameof(EnumerableOnlyReturnValues))] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A collection of whatever.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Enumerable_Task_method(
            [Values("returns", "value")] string xmlTag,
            [Values("", " ")] string space,
            [ValueSource(nameof(EnumerableTaskReturnValues))] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A task that represents the asynchronous operation. The value of the <see cref=""System.Threading.Tasks.Task{TResult}.Result" + space + @"/> parameter contains a collection of whatever.
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_method(
            [Values("returns", "value")] string xmlTag,
            [Values("A whatever", "An whatever", "The whatever")] string comment,
            [ValueSource(nameof(EnumerableReturnValues))] string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;
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

        protected override string GetDiagnosticId() => MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer();

        private static IEnumerable<string> EnumerableOnlyReturnValues() => new[] { "IEnumerable", "IEnumerable<int>", "IList<int>", "ICollection<int>", "List<int>", "Dictionary<int, int>", "int[]", }.ToHashSet();

        private static IEnumerable<string> EnumerableTaskReturnValues() => new[] { "Task<int[]>", "Task<IEnumerable>", "Task<List<int>>", }.ToHashSet();

        private static IEnumerable<string> EnumerableReturnValues() => EnumerableOnlyReturnValues().Concat(EnumerableTaskReturnValues()).ToHashSet();
    }
}