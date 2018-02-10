using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1003_EventHandlingMethodNamePrefixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_eventhandling_method() => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method() => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void OnWhatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void No_issue_is_reported_for_incorrectly_named_overridden_method() => No_issue_is_reported_for(@"

using System;

public class TestMe : BaseClass
{
    public override void Whatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method() => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void Whatever(object sender, EventArgs e) { }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method_2() => An_issue_is_reported_for(@"

using System;

public class TestMe
{
    public event EventHandler MyEvent;

    public void Initialize() => MyEvent += Whatever;

    public void Whatever(object sender, EventArgs e) { }
}
");

        protected override string GetDiagnosticId() => MiKo_1003_EventHandlingMethodNamePrefixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1003_EventHandlingMethodNamePrefixAnalyzer();
    }
}