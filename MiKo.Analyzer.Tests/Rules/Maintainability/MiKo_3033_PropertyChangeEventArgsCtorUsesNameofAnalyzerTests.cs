﻿using System.ComponentModel;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3033_PropertyChangeEventArgsCtorUsesNameofAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] TypeNames =
                                                     [
                                                         nameof(PropertyChangedEventArgs),
                                                         nameof(PropertyChangingEventArgs),
                                                         typeof(PropertyChangedEventArgs).FullName,
                                                         typeof(PropertyChangingEventArgs).FullName,
                                                     ];

        [Test]
        public void No_issue_is_reported_for_correct_usage_inside_property_([ValueSource(nameof(TypeNames))] string typeName) => No_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public int Something
        {
            get
            {
                return m_something;
            }

            set
            {
                var x = new " + typeName + @"(nameof(Something));
            }
        }

        private int m_something;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_incomplete_nameof_usage_([ValueSource(nameof(TypeNames))] string typeName) => No_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public int Something
        {
            get
            {
                return m_something;
            }

            set
            {
                var x = new " + typeName + @"(nameof(
            }
        }

        private int m_something;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_usage_on_field_assignment_([ValueSource(nameof(TypeNames))] string typeName) => No_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public int Something { get; set; }

        private " + typeName + " e = new " + typeName + @"(nameof(Something));
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_usage_of_inherited_property_on_field_assignment_([ValueSource(nameof(TypeNames))] string typeName) => No_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMeBase
    {
        public int Something { get; set; }
    }

    public class TestMe : TestMeBase
    {
        private " + typeName + " e = new " + typeName + @"(nameof(Something));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_string_usage_([ValueSource(nameof(TypeNames))] string typeName) => An_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public int Something
        {
            get
            {
                return m_something;
            }

            set
            {
                var x = new " + typeName + @"(""Something"");
            }
        }

        private int m_something;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nameof_usage_with_unknown_property_name_([ValueSource(nameof(TypeNames))] string typeName) => An_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        private " + typeName + " e = new " + typeName + @"(nameof(TestMe));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_non_nameof_invocation_usage_([ValueSource(nameof(TypeNames))] string typeName) => An_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public string Something { get; set; }

        private " + typeName + " e = new " + typeName + @"(() => Something);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_nameof_parameter_invocation_usage_([ValueSource(nameof(TypeNames))] string typeName) => No_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public " + typeName + " DoSomething(string paramName) => new " + typeName + @"(paramName);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_null_([ValueSource(nameof(TypeNames))] string typeName) => An_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        private " + typeName + " e = new " + typeName + @"(null);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_type_([ValueSource(nameof(TypeNames))] string typeName)
        {
            var originalCode = @"
using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public int Something { get; set; }

        private " + typeName + " e = new " + typeName + @"(""Something"");
    }
}
";

            var fixedCode = @"
using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public int Something { get; set; }

        private " + typeName + " e = new " + typeName + @"(nameof(Something));
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3033_PropertyChangeEventArgsCtorUsesNameofAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3033_PropertyChangeEventArgsCtorUsesNameofAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3033_CodeFixProvider();
    }
}