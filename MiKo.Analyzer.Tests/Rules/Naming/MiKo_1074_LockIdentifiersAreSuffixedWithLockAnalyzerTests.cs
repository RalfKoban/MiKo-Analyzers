using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1074_LockIdentifiersAreSuffixedWithLockAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_lock_([Values("syncRoot", "MyLock")] string lockName) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static readonly object " + lockName + @" = new object();

    public void DoSomething()
    {
        lock (" + lockName + @")
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_lock() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private static readonly object whatever = new object();

    public void DoSomething()
    {
        lock (whatever)
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1074_LockIdentifiersAreSuffixedWithLockAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1074_LockIdentifiersAreSuffixedWithLockAnalyzer();
    }
}