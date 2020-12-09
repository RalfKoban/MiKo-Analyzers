using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
            {
                "AreConnected",
                "IsConnected",
                "Connected",
                "HasConnectionEstablished",
                "CanBeConnected",
                nameof(string.IsNullOrEmpty),
                nameof(string.IsNullOrWhiteSpace),
                "IsSameKey",
                "IsReadOnly",
                "IsReadWrite",
                "IsWriteProtected",
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
        public void No_issue_is_reported_for_method_with_non_Boolean_return_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void IsDoingSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_with_non_Boolean_return_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int IsDoingSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Boolean_return_type_and_correct_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @"()
    {
        return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_with_Boolean_return_type_and_correct_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @" { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Boolean_return_type_and_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @"()
    {
        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_Boolean_return_type_and_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @" { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer();
    }
}