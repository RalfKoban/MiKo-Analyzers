using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3004_EqualsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_object_equals_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        if (string.Equals(""A"", ""B"")) throw new NotSupportedException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_equals_method_on_classes() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        if (object.Equals(""A"", ""B"")) throw new NotSupportedException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_equals_method_on_dynamic() => No_issue_is_reported_for(@"
using System;
using System.Windows;

public class TestMe
{
    private static bool IsUnsetValue(dynamic itemToCheck)
    {
        return Equals(itemToCheck, DependencyProperty.UnsetValue);
    }
}
");

        [TestCase("5", "4")]
        [TestCase("Guid.Empty", "new Guid()")]
        public void An_issue_is_reported_for_full_qualified_object_equals_method_on_structs(string x, string y) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = " + x + @";
        var y = " + y + @";
        if (object.Equals(x, y)) throw new NotSupportedException();
    }
}
");

        [TestCase("5", "4")]
        [TestCase("Guid.Empty", "new Guid()")]
        public void An_issue_is_reported_for_object_equals_method_on_structs(string x, string y) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = " + x + @";
        var y = " + y + @";
        if (Equals(x, y)) throw new NotSupportedException();
    }
}
");

        [TestCase("5", "4")]
        [TestCase("Guid.Empty", "new Guid()")]
        public void No_issue_is_reported_for_IEquatable_equals_method_on_structs(string x, string y) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = " + x + @";
        var y = " + y + @";
        if (x.Equals(y)) throw new NotSupportedException();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3004_EqualsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3004_EqualsAnalyzer();
    }
}