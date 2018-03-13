using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2036_PropertyDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test, Combinatorial]
        public void No_issue_is_reported_for_commented_method([ValueSource(nameof(BooleanReturnValues))] string returnType, [Values("returns", "value")] string xmlTag) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// Something.
    /// </" + xmlTag + @">
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
        public void An_issue_is_reported_for_commented_Boolean_property_with_missing_default_value(
                                                                                            [Values("returns", "value")] string xmlTag,
                                                                                            [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                            [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
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
    /// " + trueValue + @" if something happens; otherwise, " + falseValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething { get; set; }
}
");
        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_property_with_default_phrase(
                                                                                        [Values("returns", "value")] string xmlTag,
                                                                                        [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                        [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                        [Values("<see langword=\"true\"/>", "<see langword=\"false\"/>")] string defaultValue,
                                                                                        [ValueSource(nameof(BooleanReturnValues))] string returnType)
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
    public " + returnType + @" DoSomething { get; set; }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_correctly_commented_Boolean_property_with_default_phrase_and_line_break(
                                                                                                        [Values("returns", "value")] string xmlTag,
                                                                                                        [Values("<see langword=\"true\" />", "<see langword=\"true\"/>")] string trueValue,
                                                                                                        [Values("<see langword=\"false\" />", "<see langword=\"false\"/>")] string falseValue,
                                                                                                        [Values("<see langword=\"true\"/>", "<see langword=\"false\"/>")] string defaultValue,
                                                                                                        [ValueSource(nameof(BooleanReturnValues))] string returnType)
            => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    /// <" + xmlTag + @">
    /// " + trueValue + @" if something happens; otherwise, " + falseValue + @"
    /// The default is " + defaultValue + @".
    /// </" + xmlTag + @">
    public " + returnType + @" DoSomething(object o) => null;
}
");

        protected override string GetDiagnosticId() => MiKo_2036_PropertyDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2036_PropertyDefaultPhraseAnalyzer();

        private static IEnumerable<string> BooleanReturnValues() => new[] { "bool", "Boolean", "System.Boolean", nameof(System.Boolean), }.ToHashSet();
    }
}