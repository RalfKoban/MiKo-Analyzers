using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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
        public void No_issue_is_reported_for_property_that_returns_non_generic() => No_issue_is_reported_for(@"
public class TestMe
{
    public object DoSomething { get; set; }
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

        protected override string GetDiagnosticId() => MiKo_3217_DoNotUseGenericsWithGenericsWithGenericsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3217_DoNotUseGenericsWithGenericsWithGenericsAnalyzer();
    }
}