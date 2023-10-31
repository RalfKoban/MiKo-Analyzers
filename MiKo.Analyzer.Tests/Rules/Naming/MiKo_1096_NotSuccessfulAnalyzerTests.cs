using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1096_NotSuccessfulAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidNames =
                                                      {
                                                          "DoSomething",
                                                          "RunSuccessfully",
                                                      };

        private static readonly string[] InvalidNames =
                                                        {
                                                            "RunNotSuccesfully",
                                                            "Run_not_successfully",
                                                            "IsNotSuccesful",
                                                        };

        [Test]
        public void No_issue_is_reported_for_class_([ValueSource(nameof(ValidNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe" + name + @"
{
    public void DoSomething()
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_method_([ValueSource(nameof(ValidNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void " + name + @"()
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_property_([ValueSource(nameof(ValidNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool " + name + @" { get; set; }
}");

        [Test]
        public void No_issue_is_reported_for_event_([ValueSource(nameof(ValidNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler " + name + @";
}");

        [Test]
        public void No_issue_is_reported_for_field_([ValueSource(nameof(ValidNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_" + name + @";
}");

        [Test]
        public void An_issue_is_reported_for_class_([ValueSource(nameof(InvalidNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe" + name + @"
{
    public void DoSomething()
    {
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_([ValueSource(nameof(InvalidNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void " + name + @"()
    {
    }
}");

        [Test]
        public void An_issue_is_reported_for_property_([ValueSource(nameof(InvalidNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool " + name + @" { get; set; }
}");

        [Test]
        public void An_issue_is_reported_for_event_([ValueSource(nameof(InvalidNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler " + name + @";
}");

        [Test]
        public void An_issue_is_reported_for_field_([ValueSource(nameof(InvalidNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool m_" + name + @";
}");

        protected override string GetDiagnosticId() => MiKo_1096_NotSuccessfulAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1096_NotSuccessfulAnalyzer();
    }
}
