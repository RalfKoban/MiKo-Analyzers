﻿using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public partial class MiKo_1063_AbbreviationsInNameAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_special_name_([Values("paramName")] string specialName) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(string " + specialName + @")
        {
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_Json_constructor_([ValueSource(nameof(BadPrefixes))] string name) => No_issue_is_reported_for(@"
using System;
using System.Text.Json.Serialization;

namespace Bla
{
    public class TestMe
    {
        [JsonConstructor]
        public TestMe(int " + name + @"Value)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Newtonsoft_Json_constructor_([ValueSource(nameof(BadPrefixes))] string name) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        [Newtonsoft.Json.JsonConstructorAttribute]
        public TestMe(int " + name + @"Value)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_extern_method_([ValueSource(nameof(BadPrefixes))] string part) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public static extern int " + part + @"DoSomething();
    }
}");

        [Test]
        public void No_issue_is_reported_for_parameters_of_extern_method_([ValueSource(nameof(BadPrefixes))] string part) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public static extern int DoSomething(int " + part + @"Parameter);
    }
}");

        [Test]
        public void An_issue_is_reported_for_local_variable_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var " + prefix + @"Variable = 42;
            return " + prefix + @"Variable;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_local_foreach_variable_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(int[] variables)
        {
            foreach (var " + prefix + @"Variable in variables)
            {
                return " + prefix + @"Variable;
            }
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_field_with_prefix_([Values("", "_", "m_", "s_", "t_")] string fieldMarker, [ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int " + fieldMarker + prefix + @"Field = 42;
    }
}");

        [Test]
        public void An_issue_is_reported_for_enum_member_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public enum TestMe
    {
        None = 0,
        " + prefix.ToUpperCaseAt(0) + @"_Something = 1,
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int " + prefix + @"Property { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_event_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler " + prefix + @"Event { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_parameter_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int " + prefix + @"Parameter) { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void " + prefix + @"Method() { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_with_uppercase_starting_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void " + prefix.ToUpperCaseAt(0) + @"Method() { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class " + prefix + @"Class
    { }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_uppercase_starting_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class " + prefix.ToUpperCaseAt(0) + @"Class
    { }
}");

        [Test]
        public void An_issue_is_reported_for_namespace_with_prefix_([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace " + prefix + @"Namespace
{
    public class TestMe
    { }
}");

        [TestCase("seq", "sequential")]
        public void Code_gets_fixed_for_prefix_(string originalTerm, string fixedTerm)
        {
            const string Template = @"
using System;

public class TestMe(int ###Term)
{
}
";

            VerifyCSharpFix(Template.Replace("###", originalTerm), Template.Replace("###", fixedTerm));
        }
    }
}