using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3003_EventSignatureAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_EventHandler() => No_issue_is_reported_for(@"

public class TestMe
{
    public event EventHandler MyEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_generic_EventHandler() => No_issue_is_reported_for(@"

public class MyEventArgs : EventArgs
{
}

public class TestMe
{
    public event EventHandler<MyEventArgs> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_Action() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Action_with_1_Argument() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action<MyEventArgs> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Action_with_correct_2_Arguments() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action<object, MyEventArgs> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Action_with_incorrect_2_Arguments() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action<string, MyEventArgs> MyEvent;
}
");

        [Test]
        public void An_issue_is_reported_for_generic_Action_with_3_Arguments() => An_issue_is_reported_for(@"

public class TestMe
{
    public event Action<object, MyEventArgs, int> MyEvent;
}
");

        protected override string GetDiagnosticId() => MiKo_3003_EventSignatureAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3003_EventSignatureAnalyzer();
    }
}