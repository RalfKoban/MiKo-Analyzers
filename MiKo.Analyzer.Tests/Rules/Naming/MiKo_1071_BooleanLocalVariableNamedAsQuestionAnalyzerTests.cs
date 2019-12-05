using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1071_BooleanLocalVariableNamedAsQuestionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
            {
                "areConnected",
                "isConnected",
                "connected",
                "hasConnectionEstablished",
                "canBeConnected",
            };

        private static readonly string[] WrongNames =
            {
                "isConnectionPossible",
                "areDevicesConnected",
            };

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_non_Boolean_variable() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Boolean_variable_with_correct_name([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        bool " + name + @" = false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_var_Boolean_variable_with_correct_name([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        var " + name + @" = true;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Boolean_variable_with_incorrect_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        bool " + name + @" = true;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_Boolean_variable_with_incorrect_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        var " + name + @" = true;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_variable_declaration_pattern_for_Boolean_variable_with_correct_name([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case bool " + name + @": return;
            default: return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_variable_declaration_pattern_for_Boolean_variable_with_incorrect_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case bool " + name + @": return;
            default: return;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1071_BooleanLocalVariableNamedAsQuestionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1071_BooleanLocalVariableNamedAsQuestionAnalyzer();
    }
}