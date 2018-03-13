using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method([ValueSource(nameof(BooleanReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property([ValueSource(nameof(BooleanReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a([Values("returns", "value")] string xmlTag,
                                                                   [Values("void", "int", "Task", "Task<int>", "Task<string>")] string returnType) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method(
            [Values("returns", "value")] string xmlTag,
            [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
            [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
            [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + @" if something happens; otherwise, " + falseValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");
        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method_with_default_phrase(
            [Values("returns", "value")] string xmlTag,
            [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
            [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
            [Values("<see langword=\"true\"/>", "<see langword=\"false\"/>")] string defaultValue,
            [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + @" if something happens; otherwise, " + falseValue + ". The default is " + defaultValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method_with_line_break(
            [Values("returns", "value")] string xmlTag,
            [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
            [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
            [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + @" if something happens;
    /// otherwise, " + falseValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_Task_method(
            [Values("returns", "value")] string xmlTag,
            [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
            [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
            [ValueSource(nameof(BooleanTaskReturnValues))] string returnType) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A task that will complete with a result of " + trueValue + @" if something happens, otherwise with a result of "+ falseValue +@".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_method(
            [Values("returns", "value")] string xmlTag,
            [Values("A whatever", "An whatever", "The whatever")] string comment,
            [ValueSource(nameof(BooleanReturnValues))] string returnType) => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer();

        private static IEnumerable<string> BooleanOnlyReturnValues() => new[] { "bool", "Boolean", "System.Boolean", nameof(System.Boolean), }.ToHashSet();

        private static IEnumerable<string> BooleanTaskReturnValues() => new[] { "Task<bool>", "Task<Boolean>", "Task<System.Boolean>", }.ToHashSet();

        private static IEnumerable<string> BooleanReturnValues() => BooleanOnlyReturnValues().Concat(BooleanTaskReturnValues()).ToHashSet();
    }
}