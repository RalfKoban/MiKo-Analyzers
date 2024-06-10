using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5014_MethodReturnsEmptyListAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ProblematicReturnTypes = ["IReadOnlyList", "IReadOnlyCollection"];

        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [TestCase("int")]
        [TestCase("IEnumerable<int>")]
        [TestCase("List<int>")]
        [TestCase("Dictionary<int, int>")]
        [TestCase("IList<int>")]
        [TestCase("IDictionary<int, int>")]
        [TestCase("ICollection<int>")]
        public void No_issue_is_reported_for_an_expression_body_method_that_returns_a_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @" DoSomething() => null;
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void No_issue_is_reported_for_an_expression_body_method_that_returns_a_list_with_a_non_default_capacity_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() => new List<int>(42);
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void No_issue_is_reported_for_an_expression_body_method_that_returns_a_list_with_a_non_empty_initializer_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() => new List<int> { 42 };
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_an_expression_body_method_that_returns_an_empty_list_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() => new List<int>();
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_an_expression_body_method_that_returns_a_list_with_an_empty_initializer_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() => new List<int> { };
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_an_expression_body_conditional_method_that_returns_an_empty_list_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething(bool flag) => flag ? new List<int>() : new List<int>(42);
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_an_expression_body_conditional_method_that_returns_a_list_with_an_empty_initializer_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() => flag ? new List<int> { } : new List<int> { 42 };
}
");

        [TestCase("int")]
        [TestCase("IEnumerable<int>")]
        [TestCase("List<int>")]
        [TestCase("Dictionary<int, int>")]
        [TestCase("IList<int>")]
        [TestCase("IDictionary<int, int>")]
        [TestCase("ICollection<int>")]
        public void No_issue_is_reported_for_a_method_that_returns_a_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @" DoSomething() { return null; }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void No_issue_is_reported_for_a_method_that_returns_a_list_with_a_non_default_capacity_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return new List<int>(42); }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void No_issue_is_reported_for_a_that_returns_a_list_with_a_non_empty_initializer_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return new List<int> { 42 }; }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_a_method_that_returns_an_empty_list_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return new List<int>(); }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_a_method_that_returns_a_list_with_an_empty_initializer_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return new List<int> { }; }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_a_conditional_method_that_returns_an_empty_list_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething(bool flag) { return flag ? new List<int>() : new List<int>(42); }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_a_conditional_method_that_returns_a_list_with_an_empty_initializer_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return flag ? new List<int> { } : new List<int> { 42 }; }
}
");

        [Test]
        public void Code_gets_fixed_for_a_method_that_returns_a_list_with_an_empty_initializer_(
                                                                                            [ValueSource(nameof(ProblematicReturnTypes))] string returnType,
                                                                                            [Values("new List<int> { }", "new List<int>()")] string creation)
        {
            var template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return ###; }
}
";

            VerifyCSharpFix(template.Replace("###", creation), template.Replace("###", "Array.Empty<int>()"));
        }

        [Test]
        public void Code_gets_fixed_for_a_method_that_returns_a_list_with_an_empty_initializer_spanning_multiple_lines_(
                                                                                                                    [ValueSource(nameof(ProblematicReturnTypes))] string returnType,
                                                                                                                    [Values("new List<int> { }", "new List<int>()")] string creation)
        {
            var template = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return
                                                        ###; }
}
";

            VerifyCSharpFix(template.Replace("###", creation), template.Replace("###", "Array.Empty<int>()"));
        }

        protected override string GetDiagnosticId() => MiKo_5014_MethodReturnsEmptyListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5014_MethodReturnsEmptyListAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5014_CodeFixProvider();
    }
}