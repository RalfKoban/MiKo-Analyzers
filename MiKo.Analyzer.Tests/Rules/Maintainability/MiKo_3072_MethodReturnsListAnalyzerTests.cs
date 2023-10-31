using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3072_MethodReturnsListAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_void_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_public_method_that_returns_a_struct() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public int DoSomething() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_public_method_that_returns_a_class() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public TestMe DoSomething(int i) => new TestMe();
}
");

        [Test]
        public void No_issue_is_reported_for_public_method_that_returns_an_IEnumerable() => No_issue_is_reported_for(@"
using System;
using System.Collections;

public class TestMe
{
    public IEnumerable DoSomething(int i) => new int[0];
}
");

        [Test]
        public void No_issue_is_reported_for_public_method_that_returns_a_generic_IEnumerable() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<int> DoSomething(int i) => new int[0];
}
");

        [Test]
        public void No_issue_is_reported_for_public_method_that_returns_a_generic_IList() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IList<int> DoSomething(int i) => new List<int>();
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_a_generic_List_if_visible_as_([Values("public", "internal", "protected", "protected internal")] string visibility) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    " + visibility + @" List<int> DoSomething(int i) => new List<int>();
}
");

        [Test]
        public void No_issue_is_reported_for_private_method_that_returns_a_generic_List() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private List<int> DoSomething(int i) => new List<int>();
}
");

        [Test]
        public void No_issue_is_reported_for_public_method_that_returns_a_generic_IDictionary() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IDictionary<int, string> DoSomething(int i) => new Dictionary<int, string>();
}
");

        [Test]
        public void An_issue_is_reported_for_method_that_returns_a_generic_Dictionary_if_visible_as_([Values("public", "internal", "protected", "protected internal")] string visibility) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    " + visibility + @" Dictionary<int, string> DoSomething(int i) => new Dictionary<int, string>();
}
");

        [Test]
        public void No_issue_is_reported_for_private_method_that_returns_a_generic_Dictionary() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private Dictionary<int, string> DoSomething(int i) => new Dictionary<int, string>();
}
");

        [Test]
        public void No_issue_is_reported_for_overridden_method_that_returns_a_generic_Dictionary()
        {
            const string Code = @"
using System;
using System.Collections.Generic;

public class TestMeBase
{
    public virtual Dictionary<int, string> DoSomething(int i) => new Dictionary<int, string>();
}

public class TestMe : TestMeBase
{
    public override Dictionary<int, string> DoSomething(int i) => new Dictionary<int, string>();
}
";

            An_issue_is_reported_for(Code); // it's only a single issue on the base class
        }

        [Test]
        public void No_issue_is_reported_for_interface_implementation_method_that_returns_a_generic_Dictionary()
        {
            const string Code = @"
using System;
using System.Collections.Generic;

public interface ITestMe
{
    Dictionary<int, string> DoSomething(int i);
}

public class TestMe : ITestMe
{
    public Dictionary<int, string> DoSomething(int i) => new Dictionary<int, string>();
}
";

            An_issue_is_reported_for(Code); // it's only a single issue on the interface
        }

        protected override string GetDiagnosticId() => MiKo_3072_MethodReturnsListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3072_MethodReturnsListAnalyzer();
    }
}