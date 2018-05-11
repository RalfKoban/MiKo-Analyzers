using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method([ValueSource(nameof(EnumReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property([ValueSource(nameof(EnumReturnValues))] string returnType) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Enum_only_method(
            [Values("returns", "value")] string xmlTag,
            [ValueSource(nameof(EnumOnlyReturnValues))] string returnType) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Enum_Task_method(
            [Values("returns", "value")] string xmlTag,
            [Values("", " ")] string space,
            [ValueSource(nameof(EnumTaskReturnValues))] string returnType) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_wrong_commented_method(
            [Values("returns", "value")] string xmlTag,
            [Values("A whatever", "An whatever", "The whatever")] string comment,
            [ValueSource(nameof(EnumReturnValues))] string returnType) => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2034_EnumReturnTypeDefaultPhraseAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> EnumOnlyReturnValues() => new[] { "StringComparison", "System.StringComparison", nameof(System.StringComparison), }.ToHashSet();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> EnumTaskReturnValues() => new[] { "Task<StringComparison>", "Task<System.StringComparison>", }.ToHashSet();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> EnumReturnValues() => EnumOnlyReturnValues().Concat(EnumTaskReturnValues()).ToHashSet();
    }
}