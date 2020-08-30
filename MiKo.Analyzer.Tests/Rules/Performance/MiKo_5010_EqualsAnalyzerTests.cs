using System;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5010_EqualsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValueTypes =
            {
                "bool",
                "char",
                "short",
                "int",
                "long",
                "ushort",
                "uint",
                "ulong",
                "byte",
                "sbyte",
                nameof(Boolean),
                nameof(Char),
                nameof(Int16),
                nameof(Int32),
                nameof(Int64),
                nameof(UInt16),
                nameof(UInt32),
                nameof(UInt64),
                nameof(Byte),
                nameof(SByte),
                nameof(Guid),
                nameof(AttributeTargets),
            };

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
        public void An_issue_is_reported_for_full_qualified_object_equals_method_on_structs_(string x, string y) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_equals_method_on_structs_(string x, string y) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_IEquatable_equals_method_on_structs_(string x, string y) => No_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_object_equals_method_on_field_structs_([ValueSource(nameof(ValueTypes))] string returnType) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private " + returnType + @" _isSomething;

    public " + returnType + @" IsSomething
    {
        get { return _isSomething; }
        protected set
        {
            if (Equals(_isSomething, value))
                return;
            _isSomething = value;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_object_equals_method_on_Method_structs_([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private " + returnType + @" DoSomething();

    public " + returnType + @" IsSomething
    {
        get { throw new NotSupportedException(); }
        protected set
        {
            if (Equals(DoSomething(), value))
                return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_object_equals_method_on_inlined_Func_structs_([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + returnType + @" IsSomething
    {
        get { throw new NotSupportedException(); }
        protected set
        {
            if (Equals((Func<bool>)(() => true), value))
                return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_object_equals_method_on_Func_structs_([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public " + returnType + @" IsSomething
    {
        get { throw new NotSupportedException(); }
        protected set
        {
            Func<bool> something = () => true;
            if (Equals(something(), value))
                return;
        }
    }
}
");

        [TestCase("==")]
        [TestCase("!=")]
        [TestCase(">=")]
        [TestCase("<=")]
        [TestCase("+")]
        [TestCase("-")]
        [TestCase("whatever")]
        public void No_issue_is_reported_for_operator_(string operatorName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public static bool operator " + operatorName + @" (TestMe left, TestMe right) => Equals(left, right);
}
");

        [TestCase(
             "using System; class TestMe { void Do(Guid x, Guid y) { if (object.Equals(x, y)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x, Guid y) { if (x == y) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (object.Equals(x, Guid.Empty)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x == Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x == Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(int x) { if (object.Equals(x, 42)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(int x) { if (x == 42) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (!Equals(x, Guid.Empty)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (!(Equals(x, Guid.Empty))) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty) == false) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty) == true) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x == Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty) != false) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x == Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty) != true) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x, bool b) { if (Equals(x, Guid.Empty) && b) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x, bool b) { if (x == Guid.Empty && b) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x, bool b) { if (b && Equals(x, Guid.Empty)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x, bool b) { if (b && x == Guid.Empty) throw new NotSupportedException(); } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_5010_EqualsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5010_EqualsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5010_CodeFixProvider();
    }
}