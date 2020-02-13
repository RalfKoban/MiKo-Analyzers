using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1080_UseNumbersInsteadOfWordingAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
            {
                "antimone",
                "bone",
                "Bone",
                "component",
                "Component",
                "cone",
                "Cone",
                "done",
                "Done",
                "exceptionEnding",
                "firstWord",
                "gone",
                "Gone",
                "height",
                "ImportWorkflow",
                "InfoNeeded",
                "InvocationExpression",
                "IsEventArgs",
                "lovely",
                "Lovely",
                "Network",
                "none",
                "None",
                "NONE",
                "OnEntry",
                "OneTime",
                "OnExit",
                "oxone",
                "phone",
                "sone",
                "Sone",
                "tone",
                "Tone",
                "validOnes",
                "versioned",
                "WaitOne",
                "weight",
                "zone",
                "Zone",
            };

        private static readonly string[] WrongNames =
            {
                "componentOne",
                "componentTwo",
                "issueThree",
                "issueFour",
                "fiveTickets",
                "sixTickets",
                "sevenMiles",
                "eightHours",
                "ninePalms",
                "fortyTwo",
                "FiftyShadesOfBlue",
                "OceanThirteen",
            };

        [Test]
        public void No_issue_is_reported_for_correctly_named_namespace([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

namespace Bla" + name + @"
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_type([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe" + name + @"
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Do" + name + @"Something()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method_parameter([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int my" + name + @"Param)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_property([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Do" + name + @"Something { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_field([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string _" + name + @";
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_event([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Do" + name + @"Something;
}
");

        [Test]
        public void No_issue_is_reported_for_variable_with_correct_name([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        var " + name + @" = 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_pattern_with_correct_name([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_variable_in_foreach_loop_with_incorrect_name([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object[] values)
    {
        foreach (var " + name + @" in values)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_correctly_named_namespace([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

namespace Bla" + name + @"
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_type([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe" + name + @"
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Do" + name + @"Something()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_correctly_named_method_parameter([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int my" + name + @"Param)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_property([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Do" + name + @"Something { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_field([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string _" + name + @";
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_event([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Do" + name + @"Something;
}
");

        [Test]
        public void An_issue_is_reported_for_variable_with_incorrect_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        bool " + name + @" = true;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_pattern_with_incorrect_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

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
        public void An_issue_is_reported_for_variable_in_foreach_loop_with_incorrect_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object[] values)
    {
        foreach (var " + name + @" in values)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_constant_with_incorrect_name([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const string " + name + @" = ""something"";
}
");

        protected override string GetDiagnosticId() => MiKo_1080_UseNumbersInsteadOfWordingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1080_UseNumbersInsteadOfWordingAnalyzer();
    }
}