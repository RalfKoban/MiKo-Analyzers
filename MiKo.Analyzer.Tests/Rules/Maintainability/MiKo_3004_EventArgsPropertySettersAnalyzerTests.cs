﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3004_EventArgsPropertySettersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_EventArgs() => No_issue_is_reported_for(@"
public class TestMe
{
    public int Bla1 { get; set; }

    public int Bla2 { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_EventArgs_with_readonly_property() => No_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
    public int Bla { get; }
}
");

        [Test]
        public void No_issue_is_reported_for_EventArgs_with_property_and_private_setter() => No_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
    public int Bla { get; private set; }
}
");

        [TestCase("")]
        [TestCase("internal")]
        [TestCase("protected")]
        public void An_issue_is_reported_for_EventArgs_with_property_and_private_setter(string visibility) => An_issue_is_reported_for(@"
using System;

public class TestMeEventArgs : EventArgs
{
    public int Bla { get; " + visibility + @" set; }
}
");

        protected override string GetDiagnosticId() => MiKo_3004_EventArgsPropertySettersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3004_EventArgsPropertySettersAnalyzer();
    }
}