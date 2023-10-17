using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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
        public void An_issue_is_reported_for_switch_that_has_too_many_cases() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_switch_expression_that_has_too_many_cases() => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzer();
    }
}