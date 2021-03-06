﻿using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] BooleanOnlyReturnValues =
            {
                "bool",
                "Boolean",
                "System.Boolean",
            };

        private static readonly string[] BooleanTaskReturnValues =
            {
                "Task<bool>",
                "Task<Boolean>",
                "Task<System.Boolean>",
                "System.Threading.Tasks.Task<bool>",
                "System.Threading.Tasks.Task<Boolean>",
                "System.Threading.Tasks.Task<System.Boolean>",
            };

        private static readonly string[] BooleanReturnValues = BooleanOnlyReturnValues.Concat(BooleanTaskReturnValues).ToArray();

        [Test]
        public void No_issue_is_reported_for_uncommented_method_([ValueSource(nameof(BooleanReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_property_([ValueSource(nameof(BooleanReturnValues))] string returnType) => No_issue_is_reported_for(@"
public class TestMe
{
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_method_that_returns_a_(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("void", "int", "Task", "Task<int>", "Task<string>")] string returnType)
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
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method_(
                                                                                [Values("returns", "value")] string xmlTag,
                                                                                [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method_with_default_phrase_(
                                                                                                    [Values("returns", "value")] string xmlTag,
                                                                                                    [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                                    [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                                    [Values("<see langword=\"true\"/>", "<see langword=\"false\"/>")] string defaultValue,
                                                                                                    [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Boolean_only_method_with_line_break_(
                                                                                                [Values("returns", "value")] string xmlTag,
                                                                                                [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                                [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                                [ValueSource(nameof(BooleanOnlyReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_Boolean_Task_method_(
                                                                                [Values("returns", "value")] string xmlTag,
                                                                                [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                [ValueSource(nameof(BooleanTaskReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// A task that will complete with a result of " + trueValue + @" if something happens, otherwise with a result of " + falseValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_wrong_commented_method_(
                                                                [Values("returns", "value")] string xmlTag,
                                                                [Values("A whatever", "An whatever", "The whatever")] string comment,
                                                                [ValueSource(nameof(BooleanReturnValues))] string returnType)
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

        protected override string GetDiagnosticId() => MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer();
    }
}