using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public class MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_static_class_with_accessibility_([Values("public", "internal", "protected", "private")] string accessibility) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    " + accessibility + @" static class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_sealed_class_with_accessibility_([Values("public", "internal", "protected", "private")] string accessibility) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    " + accessibility + @" sealed class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_unsealed_non_static_class_with_accessibility_([Values("public", "protected")] string accessibility) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    " + accessibility + @" class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_struct_with_accessibility_([Values("public", "internal", "protected", "private")] string accessibility) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    " + accessibility + @" struct TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_sealed_private_class_that_inherits_from_unsealed_private_class() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private class Unsealed
        {
        }

        private sealed class Sealed : Unsealed
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_unsealed_non_static_internal_class() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    internal class TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_unsealed_private_class_that_is_the_only_private_class() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private class Unsealed
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_unsealed_private_class_that_inherits_from_unsealed_private_class()
        {
            const string Code = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private class Unsealed
        {
        }

        private class Sealed : Unsealed
        {
        }
    }
}
";
            An_issue_is_reported_for(Code, 1);
        }

        protected override string GetDiagnosticId() => MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzer();
    }
}