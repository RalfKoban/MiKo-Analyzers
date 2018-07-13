using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3030_EqualsAnalyzerTests : CodeFixVerifier
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

        [Test]
        public void An_issue_is_reported_for_object_equals_method_on_field_structs([ValueSource(nameof(ValueTypes))] string returnType) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_equals_method_on_Method_structs([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_equals_method_on_inlined_Func_structs([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_object_equals_method_on_Func_structs([ValueSource(nameof(ValueTypes))]string returnType) => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_3030_EqualsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3030_EqualsAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> ValueTypes() => new[]
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
                                                           "Guid",
                                                           nameof(System.Boolean),
                                                           nameof(System.Char),
                                                           nameof(System.Int16),
                                                           nameof(System.Int32),
                                                           nameof(System.Int64),
                                                           nameof(System.UInt16),
                                                           nameof(System.UInt32),
                                                           nameof(System.UInt64),
                                                           nameof(System.Byte),
                                                           nameof(System.SByte),
                                                           nameof(System.Guid),
                                                       }.ToHashSet();
    }
}