﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1017_GetSetPrefixedMethodsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidPrefixes = { string.Empty, "Get", "Set", "GetCanceled", "SetCanceled", "HasCanceled", "GetHashCode", "SetHash" };

        private static readonly string[] InvalidPrefixes = { "GetIs", "SetIs", "GetCan", "SetCan", "GetHas", "SetHas", "CanHas", "CanIs", "HasIs", "HasCan", "IsCan", "IsHas" };

        [Test]
        public void No_issue_is_reported_for_method_with_prefix_([ValueSource(nameof(ValidPrefixes))] string prefix) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + prefix + @"Something()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameterless_method_with_prefix_([ValueSource(nameof(InvalidPrefixes))] string prefix) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + prefix + @"Something()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_method_with_prefix_([ValueSource(nameof(InvalidPrefixes))] string prefix) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + prefix + @"Something(object o)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_DependencyObject_parameters() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public void GetIsEnabled(DependencyObject do)
    {
    }

    public void SetIsEnabled(DependencyObject do)
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1017_GetSetPrefixedMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1017_GetSetPrefixedMethodsAnalyzer();
    }
}