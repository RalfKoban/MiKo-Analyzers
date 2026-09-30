using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        private static readonly string[] ProblematicReturnTypes = ["IReadOnlyList", "IReadOnlyCollection", "IEnumerable"];

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
        public void No_issue_is_reported_for_a_method_that_returns_a_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @" DoSomething() { return null; }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void No_issue_is_reported_for_a_method_that_returns_a_list_without_arguments_but_with_a_non_empty_initializer_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return new List<int> { 42 }; }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void No_issue_is_reported_for_a_method_that_returns_a_list_with_empty_arguments_and_a_non_empty_initializer_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return new List<int>() { 42 }; }
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
        public void No_issue_is_reported_for_an_expression_body_method_that_returns_a_list_with_a_non_empty_initializer_(string returnType) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() => new List<int> { 42 };
}
");

        [Test]
        public void No_issue_is_reported_for_collection_expression_with_enumerable_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static IEnumerable<int> Create() => [];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_collection_expression_with_list_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => [];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_collection_initializer_and_capacity_parameter_with_list_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int>(42) { };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_collection_initializer_with_list_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int> { };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_empty_collection_initializer_with_list_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int> { 42 };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_initializer_with_capacity_parameter_and_with_list_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int>(0);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_initializer_with_collection_parameter_and_with_enumerable_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public static IEnumerable<int> Create(IEnumerable<int> values) => new List<int>(values.Select(_ => _ + _));
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_initializer_with_collection_parameter_and_with_list_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create(IEnumerable<int> values) => new List<int>(values.Select(_ => _ + _));
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_initializer_with_no_list() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int Id { get; set; }

        public static TestMe Create() => new TestMe
                                             {
                                                 Id = 42,
                                             };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_initializer_with_no_parameters_and_with_list_as_method_return_value() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int>();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_initializer_with_list_as_argument() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe(List<int> items)
    {
        public static IEnumerable<TestMe> Create() => new List<TestMe> { new TestMe(new List<int>()) };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_object_initializer_with_list_in_initializer() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe()
    {
        public List<int> Items { get; set; }

        public static IEnumerable<TestMe> Create() => new List<TestMe> { new TestMe { Items = new List<int>() } };
    }
}
");

        [Test] // this is a special case where we want to fix it as we have only a read-only collection, so the list will never get updates
        public void An_issue_is_reported_for_object_initializer_of_ReadOnlyCollection_and_list_as_argument() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bla
{
    public class TestMe()
    {
        public static IReadOnlyList<int> Create() => new ReadOnlyCollection<int>(new List<int>());
    }
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

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_a_conditional_method_that_returns_an_empty_list_(string returnType) => An_issue_is_reported_for(2, @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething(bool flag) { return flag ? new List<int>() : new List<int>(42); }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_a_method_that_returns_a_list_with_a_non_default_capacity_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return new List<int>(42); }
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
        public void An_issue_is_reported_for_a_method_that_returns_an_empty_list_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() { return new List<int>(); }
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

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_an_expression_body_conditional_method_that_returns_an_empty_list_(string returnType) => An_issue_is_reported_for(2, @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething(bool flag) => flag ? new List<int>() : new List<int>(42);
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_an_expression_body_method_that_returns_a_list_with_a_non_default_capacity_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() => new List<int>(42);
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
        public void An_issue_is_reported_for_an_expression_body_method_that_returns_an_empty_list_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public " + returnType + @"<int> DoSomething() => new List<int>();
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_empty_collection_initializer_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static " + returnType + @"<int> Create() => new List<int> { };
    }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_empty_collection_initializer_and_capacity_parameter_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static " + returnType + @"<int> Create() => new List<int>(42) { };
    }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_object_initializer_with_capacity_parameter_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static " + returnType + @"<int> Create() => new List<int>(0);
    }
}
");

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void An_issue_is_reported_for_object_initializer_with_no_parameters_(string returnType) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static " + returnType + @"<int> Create() => new List<int>();
    }
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

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void Code_gets_fixed_for_empty_collection_initializer_(string returnType)
        {
            var originalCode = $$"""
                                 using System.Collections.Generic;

                                 namespace Bla
                                 {
                                     public class TestMe
                                     {
                                         public static {{returnType}}<int> Create() => new List<int> { };
                                     }
                                 }
                                 """;

            var fixedCode = $$"""
                              using System;
                              using System.Collections.Generic;

                              namespace Bla
                              {
                                  public class TestMe
                                  {
                                      public static {{returnType}}<int> Create() => Array.Empty<int>();
                                  }
                              }
                              """;

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void Code_gets_fixed_for_empty_collection_initializer_with_comment_(string returnType)
        {
            var originalCode = $$"""
                                 using System.Collections.Generic;

                                 namespace Bla
                                 {
                                     public class TestMe
                                     {
                                         public static {{returnType}}<int> Create() => new List<int> { }; // some comment
                                     }
                                 }
                                 """;

            var fixedCode = $$"""
                              using System;
                              using System.Collections.Generic;

                              namespace Bla
                              {
                                  public class TestMe
                                  {
                                      public static {{returnType}}<int> Create() => Array.Empty<int>(); // some comment
                                  }
                              }
                              """;

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void Code_gets_fixed_for_object_initializer_with_no_parameters_(string returnType)
        {
            var originalCode = @"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static " + returnType + @"<int> Create() => new List<int>();
    }
}
";

            var fixedCode = @"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static " + returnType + @"<int> Create() => Array.Empty<int>();
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [TestCaseSource(nameof(ProblematicReturnTypes))]
        public void Code_gets_fixed_for_object_initializer_with_no_parameters_and_with_comment_(string returnType)
        {
            var originalCode = @"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static " + returnType + @"<int> Create() => new List<int>(); // some comment
    }
}
";

            var fixedCode = @"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static " + returnType + @"<int> Create() => Array.Empty<int>(); // some comment
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_5014_MethodReturnsEmptyListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5014_MethodReturnsEmptyListAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5014_CodeFixProvider();
    }
}