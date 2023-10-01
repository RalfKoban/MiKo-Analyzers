using System;
using System.Diagnostics.CodeAnalysis;

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
        public void No_issue_is_reported_for_non_object_Equals_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s1, string s2)
    {
        if (string.Equals(""A"", ""B"")) throw new NotSupportedException();

        if (string.Equals(s1, s2, StringComparison.Ordinal)) throw new NotSupportedException();

        if (s1.Equals(s2, StringComparison.Ordinal)) throw new NotSupportedException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_Equals_method_on_classes() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o1, object o2)
    {
        if (object.Equals(""A"", ""B"")) throw new NotSupportedException();

        if (object.Equals(o1, o2)) throw new NotSupportedException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_Equals_method_on_dynamic() => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_object_Equals_method_on_generic() => No_issue_is_reported_for(@"
using System;
using System.Windows;

public class TestMe<T>
{
    private static bool IsUnsetValue(T itemToCheck)
    {
        return Equals(itemToCheck, DependencyProperty.UnsetValue);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_non_object_Equals_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s1, string s2)
    {
        if (!string.Equals(""A"", ""B"")) throw new NotSupportedException();

        if (string.Equals(s1, s2, StringComparison.Ordinal) is false) throw new NotSupportedException();

        if (s1.Equals(s1, StringComparison.Ordinal) is false) throw new NotSupportedException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_object_Equals_method_on_classes() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o1, object o2)
    {
        if (!object.Equals(""A"", ""B"")) throw new NotSupportedException();

        if (object.Equals(o1, o2) is false) throw new NotSupportedException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_object_Equals_method_on_dynamic() => No_issue_is_reported_for(@"
using System;
using System.Windows;

public class TestMe
{
    private static bool IsUnsetValue(dynamic itemToCheck)
    {
        return Equals(itemToCheck, DependencyProperty.UnsetValue) is false || !Equals(itemToCheck, DependencyProperty.UnsetValue);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_negative_object_Equals_method_on_generic() => No_issue_is_reported_for(@"
using System;
using System.Windows;

public class TestMe<T>
{
    private static bool IsUnsetValue(T itemToCheck)
    {
        return Equals(itemToCheck, DependencyProperty.UnsetValue) is false || !Equals(itemToCheck, DependencyProperty.UnsetValue);
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
        public void No_issue_is_reported_for_IEquatable_equals_method_inside_equality_operator_of_class() => No_issue_is_reported_for(@"
using System;

public class TestMe : IEquatable<TestMe>
{
    public static bool operator ==(TestMe left, TestMe right) => Equals(left, right);

    public static bool operator !=(TestMe left, TestMe right) => !Equals(left, right);

    public bool Equals(TestMe other) => throw new NotImplementedException();

    public override bool Equals(object obj) => ReferenceEquals(this, obj) || Equals(obj as TestMe);

    public override int GetHashCode() => throw new NotImplementedException();
}
");

        [Test]
        public void No_issue_is_reported_for_IEquatable_equals_method_inside_equality_operator_of_struct() => No_issue_is_reported_for(@"
using System;

public struct TestMe : IEquatable<TestMe>
{
    public static bool operator ==(TestMe left, TestMe right) => left.Equals(right);

    public static bool operator !=(TestMe left, TestMe right) => !left.Equals(right);

    public bool Equals(TestMe other) => throw new NotImplementedException();

    public override bool Equals(object obj) => obj is TestMe other && Equals(other);

    public override int GetHashCode() => throw new NotImplementedException();
}
");

        [Test]
        public void No_issue_is_reported_for_object_Equals_method_with_object_and_null_as_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object o)
    {
        if (Equals(null, o)) throw new ArgumentNullException();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_object_Equals_method_with_multiple_parameters() => No_issue_is_reported_for(@"
using System;
using Microsoft.CodeAnalysis;

public class TestMe
{
    public void DoSomething(IParameterSymbol parameter, ITypeSymbol argumentType)
    {
        if (parameter.Type.Equals(argumentType, SymbolEqualityComparer.Default) is false)
        {
            throw new ArgumentException();
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_IEquatable_equals_method_inside_equality_operator_of_struct_that_invokes_object_Equals() => An_issue_is_reported_for(2, @"
using System;

public struct TestMe : IEquatable<TestMe>
{
    public static bool operator ==(TestMe left, TestMe right) => Equals(left, right);

    public static bool operator !=(TestMe left, TestMe right) => !Equals(left, right);

    public bool Equals(TestMe other) => throw new NotImplementedException();

    public override bool Equals(object obj) => obj is TestMe other && Equals(other);

    public override int GetHashCode() => throw new NotImplementedException();
}
");

        [TestCase("5", "4")]
        [TestCase("Guid.Empty", "new Guid()")]
        public void An_issue_is_reported_for_negative_IEquatable_equals_method_on_structs_(string x, string y) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = " + x + @";
        var y = " + y + @";

        if (!x.Equals(y)) throw new NotSupportedException();
    }
}
");

        [TestCase("5", "4")]
        [TestCase("Guid.Empty", "new Guid()")]
        public void An_issue_is_reported_for_negative_IEquatable_equals_method_with_parenthesis_on_structs_(string x, string y) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = " + x + @";
        var y = " + y + @";

        if (!(x.Equals(y))) throw new NotSupportedException();
    }
}
");

        [TestCase("5", "4")]
        [TestCase("Guid.Empty", "new Guid()")]
        public void An_issue_is_reported_for_negative_IEquatable_equals_method_using_pattern_on_structs_(string x, string y) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = " + x + @";
        var y = " + y + @";

        if (x.Equals(y) is false) throw new NotSupportedException();
    }
}
");

        [TestCase("5", "4")]
        [TestCase("Guid.Empty", "new Guid()")]
        public void An_issue_is_reported_for_full_qualified_object_Equals_method_on_structs_(string x, string y) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_Equals_method_on_structs_(string x, string y) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_Equals_method_on_structs_with_cast_to_object(string x, string y) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var x = " + x + @";
        var y = " + y + @";

        if (x.Equals((object)y)) throw new NotSupportedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_object_Equals_method_on_field_structs_([ValueSource(nameof(ValueTypes))] string returnType) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_Equals_method_on_Method_structs_([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_Equals_method_on_inlined_Func_structs_([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_Equals_method_on_Func_structs_([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_Enum_Equals_to_avoid_Boxing() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison value)
    {
        if (value.Equals(StringComparison.Ordinal)) throw new NotSupportedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Enum_Equals_with_cast_to_avoid_Boxing() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison value)
    {
        if (value.Equals((object)StringComparison.OrdinalIgnoreCase)) throw new NotSupportedException();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_object_Equals_method_with_struct_and_null_as_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        if (Equals(null, i)) throw new ArgumentNullException();
    }
}
");

        [TestCase(
             "using System; class TestMe { void Do(Guid x, Guid y) { if (object.Equals(x, y)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x, Guid y) { if (x == y) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x, bool b) { if (b && Equals(x, Guid.Empty)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x, bool b) { if (b && x == Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x, bool b) { if (Equals(x, Guid.Empty) && b) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x, bool b) { if (x == Guid.Empty && b) throw new NotSupportedException(); } }")]
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
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty) is false) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty) is true) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x == Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty) != false) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x == Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (Equals(x, Guid.Empty) != true) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (!(x.Equals(Guid.Empty))) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (!x.Equals(Guid.Empty)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(Guid x) { if (x.Equals(Guid.Empty) is false) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(Guid x) { if (x != Guid.Empty) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(StringComparison x) { if (Equals(x, StringComparison.Ordinal)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(StringComparison x) { if (x == StringComparison.Ordinal) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(StringComparison x) { if (!Equals(x, StringComparison.Ordinal)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(StringComparison x) { if (x != StringComparison.Ordinal) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(StringComparison x) { if (!(Equals(x, StringComparison.Ordinal))) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(StringComparison x) { if (x != StringComparison.Ordinal) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(StringComparison x) { if (Equals(x, StringComparison.Ordinal) != true) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(StringComparison x) { if (x != StringComparison.Ordinal) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(StringComparison x) { if (Equals(x, StringComparison.Ordinal) is false) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(StringComparison x) { if (x != StringComparison.Ordinal) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(StringComparison x, bool b) { if (b && Equals(x, StringComparison.Ordinal)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(StringComparison x, bool b) { if (b && x == StringComparison.Ordinal) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(StringComparison x) { if (x.Equals((object)StringComparison.Ordinal)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(StringComparison x) { if (x == StringComparison.Ordinal) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(StringComparison x) { if (x.Equals(StringComparison.Ordinal)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(StringComparison x) { if (x == StringComparison.Ordinal) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(int x) { if (!(x.Equals(42))) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(int x) { if (x != 42) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(int x) { if (!x.Equals(42)) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(int x) { if (x != 42) throw new NotSupportedException(); } }")]
        [TestCase(
             "using System; class TestMe { void Do(int x) { if (x.Equals(42) is false) throw new NotSupportedException(); } }",
             "using System; class TestMe { void Do(int x) { if (x != 42) throw new NotSupportedException(); } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [Test]
        public void Code_gets_fixed_for_IEquatable_equals_method_inside_equality_operator_of_struct_that_invokes_object_Equals()
        {
            const string OriginalCode = @"
using System;

public struct TestMe : IEquatable<TestMe>
{
    public static bool operator ==(TestMe left, TestMe right) => Equals(left, right);

    public static bool operator !=(TestMe left, TestMe right) => !Equals(left, right);

    public bool Equals(TestMe other) => throw new NotImplementedException();

    public override bool Equals(object obj) => obj is TestMe other && Equals(other);

    public override int GetHashCode() => throw new NotImplementedException();
}
";

            const string FixedCode = @"
using System;

public struct TestMe : IEquatable<TestMe>
{
    public static bool operator ==(TestMe left, TestMe right) => left.Equals(right);

    public static bool operator !=(TestMe left, TestMe right) => left.Equals(right) is false;

    public bool Equals(TestMe other) => throw new NotImplementedException();

    public override bool Equals(object obj) => obj is TestMe other && Equals(other);

    public override int GetHashCode() => throw new NotImplementedException();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_negative_Equals_on_struct_and_keeps_indentation()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool SomeProperty { get; set; }

    public void DoSomething(string s, StringComparison comparison)
    {
        SomeProperty = string.IsNullOrEmpty(s) &&
                       !Equals(comparison, StringComparison.Ordinal);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool SomeProperty { get; set; }

    public void DoSomething(string s, StringComparison comparison)
    {
        SomeProperty = string.IsNullOrEmpty(s) &&
                       comparison != StringComparison.Ordinal;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_false_pattern_Equals_on_struct_and_keeps_indentation()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public bool SomeProperty { get; set; }

    public void DoSomething(string s, StringComparison comparison)
    {
        SomeProperty = string.IsNullOrEmpty(s) &&
                       Equals(comparison, StringComparison.Ordinal) is false;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public bool SomeProperty { get; set; }

    public void DoSomething(string s, StringComparison comparison)
    {
        SomeProperty = string.IsNullOrEmpty(s) &&
                       comparison != StringComparison.Ordinal;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_5010_EqualsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5010_EqualsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5010_CodeFixProvider();
    }
}