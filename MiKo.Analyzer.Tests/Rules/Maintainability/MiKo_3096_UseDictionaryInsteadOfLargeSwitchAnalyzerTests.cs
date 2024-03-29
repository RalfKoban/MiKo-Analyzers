﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_switch_that_has_too_few_cases() => No_issue_is_reported_for(@"
using System;

public enum TestMeKind
{
    None = 0,
    Some = 1,
    Any = 2,
}

public class TestMe
{
    public StringComparison Map(TestMeKind kind)
    {
        switch (kind)
        {
            case TestMeKind.None: return StringComparison.Ordinal;
            case TestMeKind.Some: return StringComparison.OrdinalIgnoreCase;
            case TestMeKind.Any: return StringComparison.CurrentCulture;
            default: return StringComparison.InvariantCulture;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_expression_that_has_too_few_cases() => No_issue_is_reported_for(@"
using System;

public enum TestMeKind
{
    None = 0,
    Some = 1,
    Any = 2,
}

public class TestMe
{
    public StringComparison Map(TestMeKind kind) => kind switch
                                                                {
                                                                    TestMeKind.None => StringComparison.Ordinal,
                                                                    TestMeKind.Some => StringComparison.OrdinalIgnoreCase,
                                                                    TestMeKind.Any => StringComparison.CurrentCulture,
                                                                    _ => StringComparison.InvariantCulture,
                                                                };
}
");

        [Test]
        public void No_issue_is_reported_for_switch_that_has_too_many_cases_but_invokes_some_methods() => No_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.None: return DoSomething();
            case MappingKind.Value0: return MappedKind.Value0;
            case MappingKind.Value1: return MappedKind.Value1;
            case MappingKind.Value2: return MappedKind.Value2;
            case MappingKind.Value3: return MappedKind.Value3;
            case MappingKind.Value4: return MappedKind.Value4;
            case MappingKind.Value5: return MappedKind.Value5;
            case MappingKind.Value6: return MappedKind.Value6;
            case MappingKind.Value7: return MappedKind.Value7;
            case MappingKind.Value8: return MappedKind.Value8;
            case MappingKind.Value9: return MappedKind.Value9;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private MappedKind DoSomething() => MappedKind.None;
}
");

        [Test]
        public void No_issue_is_reported_for_switch_expression_that_has_too_many_cases_but_invokes_some_methods() => No_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind) => kind switch
                                                           {
                                                               MappingKind.None => DoSomething(),
                                                               MappingKind.Value0 => MappedKind.Value0,
                                                               MappingKind.Value1 => MappedKind.Value1,
                                                               MappingKind.Value2 => MappedKind.Value2,
                                                               MappingKind.Value3 => MappedKind.Value3,
                                                               MappingKind.Value4 => MappedKind.Value4,
                                                               MappingKind.Value5 => MappedKind.Value5,
                                                               MappingKind.Value6 => MappedKind.Value6,
                                                               MappingKind.Value7 => MappedKind.Value7,
                                                               MappingKind.Value8 => MappedKind.Value8,
                                                               MappingKind.Value9 => MappedKind.Value9,
                                                               _ => throw new ArgumentOutOfRangeException(),
                                                           };

    private MappedKind DoSomething() => MappedKind.None;
}
");

        [Test]
        public void No_issue_is_reported_for_incomplete_switch_that_has_too_many_cases() => No_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.None: return MappedKind.None;
            case MappingKind.Value0: return MappedKind.None;
            case MappingKind.Value1: return MappedKind.None;
            case MappingKind.Value2: return MappedKind.None;
            case MappingKind.Value3: return MappedKind.None;
            case MappingKind.Value4: return MappedKind.None;
            case MappingKind.Value5: return MappedKind.None;
            case MappingKind.Value6: return MappedKind.None;
            case MappingKind.Value7: return MappedKind.None;
            case MappingKind.Value8: return MappedKind.None;
            case MappingKind.Value9: return
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incomplete_switch_expression_that_has_too_many_cases() => No_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind) => kind switch
                                                           {
                                                               MappingKind.None => MappedKind.None,
                                                               MappingKind.Value0 => MappedKind.None,
                                                               MappingKind.Value1 => MappedKind.None,
                                                               MappingKind.Value2 => MappedKind.None,
                                                               MappingKind.Value3 => MappedKind.None,
                                                               MappingKind.Value4 => MappedKind.None,
                                                               MappingKind.Value5 => MappedKind.None,
                                                               MappingKind.Value6 => MappedKind.None,
                                                               MappingKind.Value7 => MappedKind.None,
                                                               MappingKind.Value8 => MappedKind.None,
                                                               MappingKind.Value9 => 
                                                           };

    private MappedKind DoSomething() => MappedKind.None;
}
");

        [Test]
        public void No_issue_is_reported_for_switch_with_multiple_throws() => No_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.Value0: return MappedKind.Value0;
            case MappingKind.Value1: return MappedKind.Value1;
            case MappingKind.Value2: return MappedKind.Value2;
            case MappingKind.Value3: return MappedKind.Value3;
            case MappingKind.Value4: return MappedKind.Value4;
            case MappingKind.Value5: return MappedKind.Value5;
            case MappingKind.Value6: return MappedKind.Value6;
            case MappingKind.Value7: return MappedKind.Value7;
            case MappingKind.Value8: return MappedKind.Value8;
            case MappingKind.Value9: return MappedKind.Value9;

            case MappingKind.None:
                throw new NotSupportedException();

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_with_variable_declaration_inside() => No_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.Value0: return MappedKind.Value0;
            case MappingKind.Value1: return MappedKind.Value1;
            case MappingKind.Value2: return MappedKind.Value2;
            case MappingKind.Value3: return MappedKind.Value3;
            case MappingKind.Value4: return MappedKind.Value4;
            case MappingKind.Value5: return MappedKind.Value5;
            case MappingKind.Value6: return MappedKind.Value6;
            case MappingKind.Value7: return MappedKind.Value7;
            case MappingKind.Value8: return MappedKind.Value8;
            case MappingKind.Value9: return MappedKind.Value9;

            case MappingKind.None:
            {
                var x = MappedKind.None;

                return x;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_expression_with_multiple_throws() => No_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind) => kind switch
                                                           {
                                                               MappingKind.None => throw new NotSupportedException(),
                                                               MappingKind.Value0 => MappedKind.Value0,
                                                               MappingKind.Value1 => MappedKind.Value1,
                                                               MappingKind.Value2 => MappedKind.Value2,
                                                               MappingKind.Value3 => MappedKind.Value3,
                                                               MappingKind.Value4 => MappedKind.Value4,
                                                               MappingKind.Value5 => MappedKind.Value5,
                                                               MappingKind.Value6 => MappedKind.Value6,
                                                               MappingKind.Value7 => MappedKind.Value7,
                                                               MappingKind.Value8 => MappedKind.Value8,
                                                               MappingKind.Value9 => MappedKind.Value9,
                                                               _ => throw new ArgumentOutOfRangeException(),
                                                           };
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_has_too_many_cases_and_maps_enums_to_enums() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.None: return MappedKind.None;
            case MappingKind.Value0: return MappedKind.Value0;
            case MappingKind.Value1: return MappedKind.Value1;
            case MappingKind.Value2: return MappedKind.Value2;
            case MappingKind.Value3: return MappedKind.Value3;
            case MappingKind.Value4: return MappedKind.Value4;
            case MappingKind.Value5: return MappedKind.Value5;
            case MappingKind.Value6: return MappedKind.Value6;
            case MappingKind.Value7: return MappedKind.Value7;
            case MappingKind.Value8: return MappedKind.Value8;
            case MappingKind.Value9: return MappedKind.Value9;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_has_too_many_cases_using_blocks_and_maps_enums_to_enums() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.None:   { return MappedKind.None;   }
            case MappingKind.Value0: { return MappedKind.Value0; }
            case MappingKind.Value1: { return MappedKind.Value1; }
            case MappingKind.Value2: { return MappedKind.Value2; }
            case MappingKind.Value3: { return MappedKind.Value3; }
            case MappingKind.Value4: { return MappedKind.Value4; }
            case MappingKind.Value5: { return MappedKind.Value5; }
            case MappingKind.Value6: { return MappedKind.Value6; }
            case MappingKind.Value7: { return MappedKind.Value7; }
            case MappingKind.Value8: { return MappedKind.Value8; }
            case MappingKind.Value9: { return MappedKind.Value9; }
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_that_has_too_many_cases_and_maps_enums_to_enums() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public enum MappedKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public MappedKind Map(MappingKind kind) => kind switch
                                                           {
                                                               MappingKind.None => MappedKind.None,
                                                               MappingKind.Value0 => MappedKind.Value0,
                                                               MappingKind.Value1 => MappedKind.Value1,
                                                               MappingKind.Value2 => MappedKind.Value2,
                                                               MappingKind.Value3 => MappedKind.Value3,
                                                               MappingKind.Value4 => MappedKind.Value4,
                                                               MappingKind.Value5 => MappedKind.Value5,
                                                               MappingKind.Value6 => MappedKind.Value6,
                                                               MappingKind.Value7 => MappedKind.Value7,
                                                               MappingKind.Value8 => MappedKind.Value8,
                                                               MappingKind.Value9 => MappedKind.Value9,
                                                               _ => throw new ArgumentOutOfRangeException(),
                                                           };
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_has_too_many_cases_and_maps_enums_to_literals() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public int Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.None: return -1;
            case MappingKind.Value0: return 0;
            case MappingKind.Value1: return 1;
            case MappingKind.Value2: return 2;
            case MappingKind.Value3: return 3;
            case MappingKind.Value4: return 4;
            case MappingKind.Value5: return 5;
            case MappingKind.Value6: return 6;
            case MappingKind.Value7: return 7;
            case MappingKind.Value8: return 8;
            case MappingKind.Value9: return 9;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_that_has_too_many_cases_and_maps_enums_to_literals() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public int Map(MappingKind kind) => kind switch
                                                   {
                                                       MappingKind.None => -1,
                                                       MappingKind.Value0 => 0,
                                                       MappingKind.Value1 => 1,
                                                       MappingKind.Value2 => 2,
                                                       MappingKind.Value3 => 3,
                                                       MappingKind.Value4 => 4,
                                                       MappingKind.Value5 => 5,
                                                       MappingKind.Value6 => 6,
                                                       MappingKind.Value7 => 7,
                                                       MappingKind.Value8 => 8,
                                                       MappingKind.Value9 => 9,
                                                       _ => throw new ArgumentOutOfRangeException(),
                                                   };
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_has_too_many_cases_and_maps_enums_to_predefined_type_members() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public object Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.None: return -1;
            case MappingKind.Value0: return double.MinValue;
            case MappingKind.Value1: return float.MaxValue;
            case MappingKind.Value2: return string.Empty;
            case MappingKind.Value3: return int.MinValue;
            case MappingKind.Value4: return uint.MaxValue;
            case MappingKind.Value5: return long.MinValue;
            case MappingKind.Value6: return ulong.MaxValue;
            case MappingKind.Value7: return byte.MinValue;
            case MappingKind.Value8: return sbyte.MaxValue;
            case MappingKind.Value9: return short.MinValue;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_that_has_too_many_cases_and_maps_enums_to_predefined_type_members() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public object Map(MappingKind kind) => kind switch
                                                   {
                                                       MappingKind.None => -1,
                                                       MappingKind.Value0 => double.MinValue,
                                                       MappingKind.Value1 => float.MaxValue,
                                                       MappingKind.Value2 => string.Empty,
                                                       MappingKind.Value3 => int.MinValue,
                                                       MappingKind.Value4 => uint.MaxValue,
                                                       MappingKind.Value5 => long.MinValue,
                                                       MappingKind.Value6 => ulong.MaxValue,
                                                       MappingKind.Value7 => byte.MinValue,
                                                       MappingKind.Value8 => sbyte.MaxValue,
                                                       MappingKind.Value9 => short.MinValue,
                                                       _ => throw new ArgumentOutOfRangeException(),
                                                   };
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_has_too_many_cases_and_maps_enums_to_type() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public Type Map(MappingKind kind)
    {
        switch (kind)
        {
            case MappingKind.None: return null;
            case MappingKind.Value0: return typeof(double);
            case MappingKind.Value1: return typeof(float);
            case MappingKind.Value2: return typeof(string);
            case MappingKind.Value3: return typeof(int);
            case MappingKind.Value4: return typeof(uint);
            case MappingKind.Value5: return typeof(long);
            case MappingKind.Value6: return typeof(ulong);
            case MappingKind.Value7: return typeof(byte);
            case MappingKind.Value8: return typeof(sbyte);
            case MappingKind.Value9: return typeof(short);
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_expression_that_has_too_many_cases_and_maps_enums_to_type() => An_issue_is_reported_for(@"
using System;

public enum MappingKind
{
    None = 0,
    Value0,
    Value1,
    Value2,
    Value3,
    Value4,
    Value5,
    Value6,
    Value7,
    Value8,
    Value9,
}

public class TestMe
{
    public Type Map(MappingKind kind) => kind switch
                                                   {
                                                       MappingKind.None => null,
                                                       MappingKind.Value0 => typeof(double),
                                                       MappingKind.Value1 => typeof(float),
                                                       MappingKind.Value2 => typeof(string),
                                                       MappingKind.Value3 => typeof(int),
                                                       MappingKind.Value4 => typeof(uint),
                                                       MappingKind.Value5 => typeof(long),
                                                       MappingKind.Value6 => typeof(ulong),
                                                       MappingKind.Value7 => typeof(byte),
                                                       MappingKind.Value8 => typeof(sbyte),
                                                       MappingKind.Value9 => typeof(short),
                                                       _ => throw new ArgumentOutOfRangeException(),
                                                   };
}
");

        protected override string GetDiagnosticId() => MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzer();
    }
}