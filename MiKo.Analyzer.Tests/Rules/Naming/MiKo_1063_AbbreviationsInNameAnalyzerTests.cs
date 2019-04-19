﻿using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1063_AbbreviationsInNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] BadPrefixes =
            {
                "btn",
                "cb",
                "cmd",
                "lbl",
                "mgr",
                "msg",
                // TODO: RKN "prop",
                "tmp",
                "txt",
            };

        private static readonly string[] BadPostfixes = BadPrefixes.Concat(new[]
                                                                               {
                                                                                   "BL",
                                                                                   "Bl",
                                                                                   // TODO: RKN "prop",
                                                                                    "VM",
                                                                                    "Vm",
                                                                               })
                                                                   .ToArray();

        [Test]
        public void No_issue_is_reported_for_properly_named_code() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int m_i;

        public TestMe(int i)
        {
            m_i = i;
        }

        public event EventHandler Raised;

        public string Name { get; set; }

        public int DoSomething()
        {
            var x = 42;
            return x;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_variable_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_field_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int " + prefix + @"Field = 42;
    }
}");

        [Test]
        public void An_issue_is_reported_for_property_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int " + prefix + @"Property { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_event_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler " + prefix + @"Event { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_parameter_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int " + prefix + @"Parameter) { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void " + prefix + @"Method() { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class " + prefix + @"Class
    { }
}");

        [Test]
        public void An_issue_is_reported_for_namespace_with_prefix([ValueSource(nameof(BadPrefixes))] string prefix) => An_issue_is_reported_for(@"
using System;

namespace " + prefix + @"Namespace
{
    public class TestMe
    { }
}");

        [Test]
        public void An_issue_is_reported_for_local_variable_with_postfix([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            var variable" + postfix + @" = 42;
            return variable" + postfix + @";
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_field_with_postfix([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int field" + postfix + @" = 42;
    }
}");

        [Test]
        public void An_issue_is_reported_for_property_with_postfix([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Property" + postfix + @" { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_event_with_postfix([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public event EventHandler My" + postfix + @" { get; set; }
    }
}");

        [Test]
        public void An_issue_is_reported_for_parameter_with_postfix([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int parameter" + postfix + @") { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_method_with_postfix([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void Method" + postfix + @"() { }
    }
}");

        [Test]
        public void An_issue_is_reported_for_class_with_postfix([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class Class" + postfix + @"
    { }
}");

        [Test]
        public void An_issue_is_reported_for_namespace_with_postfix([ValueSource(nameof(BadPostfixes))] string postfix) => An_issue_is_reported_for(@"
using System;

namespace Bla" + postfix + @"
{
    public class TestMe
    { }
}");

        protected override string GetDiagnosticId() => MiKo_1063_AbbreviationsInNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1063_AbbreviationsInNameAnalyzer();
    }
}