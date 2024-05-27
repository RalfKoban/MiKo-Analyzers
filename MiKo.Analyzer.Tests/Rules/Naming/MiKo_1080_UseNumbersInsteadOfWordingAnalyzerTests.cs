using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1080_UseNumbersInsteadOfWordingAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
                                                        {
                                                            "_first_",
                                                            "_one_",
                                                            "_second_",
                                                            "_third_",
                                                            "antimone",
                                                            "anyone",
                                                            "Anyone",
                                                            "bone",
                                                            "Bone",
                                                            "component",
                                                            "Component",
                                                            "cone",
                                                            "Cone",
                                                            "done",
                                                            "Done",
                                                            "everyone",
                                                            "Everyone",
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
                                                            "noOne",
                                                            "NoOne",
                                                            "OnEntry",
                                                            "OneTime",
                                                            "OnExit",
                                                            "oxone",
                                                            "phone",
                                                            "seconds",
                                                            "Seconds",
                                                            "SetupNonExistentDevice",
                                                            "someone",
                                                            "sone",
                                                            "Sone",
                                                            "tone",
                                                            "Tone",
                                                            "twoLetter",
                                                            "TwoLetter",
                                                            "validOnes",
                                                            "versioned",
                                                            "WaitOne",
                                                            "weight",
                                                            "zone",
                                                            "Zone",
                                                            "Exponent",
                                                            "exponential",
                                                            "GetDissimilarityForType",
                                                            "GetDissimilaritiesForType",
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
                                                          "firstItem",
                                                          "secondItem",
                                                          "thirdItem",
                                                          "fourthItem",
                                                          "fifthItem",
                                                          "sixthItem",
                                                          "seventhItem",
                                                          "eighthItem",
                                                          "ninthItem",
                                                          "tenthItem",
                                                      };

        [Test]
        public void No_issue_is_reported_for_correctly_named_namespace_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_named_type_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe" + name + @"
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Do" + name + @"Something()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method_parameter_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int my" + name + @"Param)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_property_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Do" + name + @"Something { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_field_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string _" + name + @";
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_event_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Do" + name + @"Something;
}
");

        [Test]
        public void No_issue_is_reported_for_variable_with_correct_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_variable_declaration_pattern_with_correct_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_variable_in_foreach_loop_with_incorrect_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_correctly_named_namespace_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

namespace Some" + name + @"
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
        public void An_issue_is_reported_for_incorrectly_named_type_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe" + name + @"
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_method_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Do" + name + @"Something()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_correctly_named_method_parameter_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int my" + name + @"Param)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_property_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void Do" + name + @"Something { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_field_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string _" + name + @";
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_event_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Do" + name + @"Something;
}
");

        [Test]
        public void An_issue_is_reported_for_variable_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_variable_declaration_pattern_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_variable_in_foreach_loop_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_constant_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const string " + name + @" = ""something"";
}
");

        [TestCase("CalculateLevenshteinDistanceOnly")]
        [TestCase("CalculateLevensteinDistanceOnly")]
        public void No_issue_is_reported_for_method_(string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int  " + name + @"() => 42;
}
");

        protected override string GetDiagnosticId() => MiKo_1080_UseNumbersInsteadOfWordingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1080_UseNumbersInsteadOfWordingAnalyzer();
    }
}