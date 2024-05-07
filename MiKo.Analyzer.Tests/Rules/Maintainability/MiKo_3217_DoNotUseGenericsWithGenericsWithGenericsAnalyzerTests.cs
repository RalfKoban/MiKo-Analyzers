using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3217_DoNotUseGenericsWithGenericsWithGenericsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_that_returns_void() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_non_generic() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_generic_with_non_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<int> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_generic_dictionary_with_non_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IDictionary<int, string> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_generic_task_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public Task<IReadOnlyList<int>> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_Action_with_generic() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public Action<IReadOnlyList<int>> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_Predicate_with_generic() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public Predicate<IReadOnlyList<int>> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_Func_with_generic() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public Func<IReadOnlyList<int>> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_Expression_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq.Expressions;

public class TestMe
{
    public Expression<IReadOnlyList<int>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<IEnumerable<int>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_generic_dictionary_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IDictionary<int, IEnumerable<int>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_generic_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<IEnumerable<IEnumerable<int>>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_generic_dictionary_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IDictionary<int, IDictionary<string, IEnumerable<int>>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_generic_task_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public Task<IReadOnlyList<IEnumerable<int>>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_Action_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Action<IReadOnlyList<IEnumerable<int>>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_Predicate_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Predicate<IReadOnlyList<IEnumerable<int>>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_Func_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Func<IReadOnlyList<IEnumerable<int>>> DoSomething() => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_Expression_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq.Expressions;

public class TestMe
{
    public Func<IReadOnlyList<IEnumerable<int>>> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_non_generic() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_generic_with_non_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(IEnumerable<int> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_generic_dictionary_with_non_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(IDictionary<int, string> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_generic_task_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(Task<IReadOnlyList<int>> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_Action_with_generic() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(Action<IReadOnlyList<int>> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_Predicate_with_generic() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(Predicate<IReadOnlyList<int>> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_Func_with_generic() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(Func<IReadOnlyList<int>> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_Expression_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq.Expressions;

public class TestMe
{
    public void DoSomething(Func<IReadOnlyList<int>> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_non_generic() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(IEnumerable<IEnumerable<int>> parameter) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_generic_dictionary_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(IDictionary<int, IEnumerable<int>> parameter) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_generic_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(IEnumerable<IEnumerable<IEnumerable<int>>> parameter) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_generic_dictionary_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(IDictionary<int, IDictionary<string, IEnumerable<int>>> parameter) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_generic_task_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public void DoSomething(Task<IReadOnlyList<IEnumerable<int>>> parameter) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_Action_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(Action<IReadOnlyList<IEnumerable<int>>> parameter) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_Predicate_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(Predicate<IReadOnlyList<IEnumerable<int>>> parameter) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_Func_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(Func<IReadOnlyList<IEnumerable<int>>> parameter) => null;
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_receives_Expression_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq.Expressions;

public class TestMe
{
    public void DoSomething(Expression<IReadOnlyList<IEnumerable<int>>> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_generic_with_non_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<int> DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_generic_dictionary_with_non_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IDictionary<int, string> DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_generic_task_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public Task<IReadOnlyList<int>> DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_Action_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Action<IReadOnlyList<int>> DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_Predicate_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Predicate<IReadOnlyList<int>> DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_Func_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Func<IReadOnlyList<int>> DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_Expression_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq.Expressions;

public class TestMe
{
    public Expression<IReadOnlyList<int>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<IEnumerable<int>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_generic_dictionary_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IDictionary<int, IEnumerable<int>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_generic_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<IEnumerable<IEnumerable<int>>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_generic_dictionary_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public IDictionary<int, IDictionary<string, IEnumerable<int>>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_generic_task_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestMe
{
    public Task<IReadOnlyList<IEnumerable<int>>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_Action_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Action<IReadOnlyList<IEnumerable<int>>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_Predicate_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Predicate<IReadOnlyList<IEnumerable<int>>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_Func_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public Func<IReadOnlyList<IEnumerable<int>>> DoSomething { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_property_that_returns_Expression_with_generic_with_generic() => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq.Expressions;

public class TestMe
{
    public Expression<IReadOnlyList<IEnumerable<int>>> DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_Mock_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

using Moq;

public class TestMe
{
    public Mock<IReadOnlyList<int>> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_returns_Mock_with_generic_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

using Moq;

public class TestMe
{
    public Mock<IReadOnlyList<IEnumerable<int>>> DoSomething() => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_Mock_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

using Moq;

public class TestMe
{
    public void DoSomething(Mock<IReadOnlyList<int>> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_receives_Mock_with_generic_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

using Moq;

public class TestMe
{
    public void DoSomething(Mock<IReadOnlyList<IEnumerable<int>>> parameter) => null;
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_Mock_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

using Moq;

public class TestMe
{
    public Mock<IReadOnlyList<int>> DoSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_that_returns_Mock_with_generic_with_generic() => No_issue_is_reported_for(@"
using System.Collections.Generic;

using Moq;

public class TestMe
{
    public Mock<IReadOnlyList<IEnumerable<int>>> DoSomething { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_3217_DoNotUseGenericsWithGenericsWithGenericsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3217_DoNotUseGenericsWithGenericsWithGenericsAnalyzer();
    }
}