using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1073_BooleanFieldNamedAsQuestionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
            {
                "AreConnected",
                "IsConnected",
                "Connected",
                "HasConnectionEstablished",
                "CanBeConnected",
            };

        private static readonly string[] WrongNames =
            {
                "IsConnectionPossible",
                "AreDevicesConnected",
            };

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_Boolean_field([Values("m_", "s_", "_")] string prefix) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int  " + prefix + @"IsDoingSomething;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_Boolean_field_with_correct_name([ValueSource(nameof(CorrectNames))] string name, [Values("m_", "s_", "_")] string prefix) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    private bool " + prefix + name + @";
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_Boolean_field_with_incorrect_name([ValueSource(nameof(WrongNames))] string name, [Values("m_", "s_", "_")] string prefix) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    private bool " + prefix + name + @";
}
");

        protected override string GetDiagnosticId() => MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer();
    }
}