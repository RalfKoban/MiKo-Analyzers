using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3236_UseAliasInsteadOfQualifiedNamesAnalyzerTests : CodeFixVerifier
    {
        [TestCase("int i")]
        [TestCase("string s")]
        public void No_issue_is_reported_for_no_fully_qualified_name_(string parameter) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(" + parameter + @") { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_fully_qualified_name_in_namespace() => No_issue_is_reported_for(@"
using System;

namespace Bla.Blubb.BlubbDiBlubb
{
    public class TestMe
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_fully_qualified_name_in_file_scoped_namespace() => No_issue_is_reported_for(@"
using System;

namespace Bla.Blubb.BlubbDiBlubb;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_fully_qualified_name_in_using_directive() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_fully_qualified_name_in_alias() => No_issue_is_reported_for(@"
using System;

using Map = System.Collections.Generic.Dictionary<string, string>;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_fully_qualified_name_in_alias_using_custom_types() => No_issue_is_reported_for(@"
using System;

using Map = System.Collections.Generic.Dictionary<Bla.TestMe, string>;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_qualified_name_of_enum() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() => DoSomething(StringComparison.Ordinal);

        public void DoSomething(StringComparison comparison) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_qualified_name_of_enum_in_case_label() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.Ordinal:
                    return 42;

                default:
                    return -1;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_qualified_name_of_nested_classes() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public static class Constants
    {
        public static class Numbers
        {
            public const int AnswerToAll = 42;
        }
    }

    public class TestMe
    {
        public void DoSomething() => DoSomething(Constants.Numbers.AnswerToAll);

        public void DoSomething(int number) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_qualified_name_of_nested_classes_in_case_label() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public static class Constants
    {
        public static class Numbers
        {
            public const int AnswerToAll = 42;
        }
    }

    public class TestMe
    {
        public void DoSomething(int number)
        {
            switch (number)
            {
                case Constants.Numbers.AnswerToAll:
                    return;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_qualified_name_in_XML_documentation() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    /// <summary>
    /// Provides stuff for <see cref=""System.Diagnostics.CodeAnalysis.SuppressMessageAttribute""/>.
    /// </summary>
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_qualified_name_in_nested_struct_used_as_method_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public struct S
    {
        public struct Nested
        {
        }
    }

    public class TestMe
    {
        public void DoSomething(S.Nested nested) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fully_qualified_name_as_method_parameter() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(System.Collections.Generic.Dictionary<string, string> map)
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3236_UseAliasInsteadOfQualifiedNamesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3236_UseAliasInsteadOfQualifiedNamesAnalyzer();
    }
}