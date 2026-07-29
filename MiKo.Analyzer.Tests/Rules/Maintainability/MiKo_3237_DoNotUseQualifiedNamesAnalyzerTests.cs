using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3237_DoNotUseQualifiedNamesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_no_fully_qualified_name() => No_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => File.ReadAllText(""some path"");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_fully_qualified_namespace_only_in_nameof() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => nameof(System.IO);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fully_qualified_namespace_with_type_in_nameof() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => nameof(System.IO.File);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fully_qualified_name_with_2_namespaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public string DoSomething() => System.IO.File.ReadAllText(""some path"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fully_qualified_name_with_3_namespaces() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public object DoSomething() => System.Collections.Generic.KeyValuePair.Create(""a"", ""b"");
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3237_DoNotUseQualifiedNamesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3237_DoNotUseQualifiedNamesAnalyzer();
    }
}