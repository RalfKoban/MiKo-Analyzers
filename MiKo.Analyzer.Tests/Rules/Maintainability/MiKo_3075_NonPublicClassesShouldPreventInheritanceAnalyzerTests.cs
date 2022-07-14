using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public class MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_abstract_class_with_accessibility_([Values("public", "internal", "protected", "private")] string accessibility) => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    " + accessibility + @" abstract class TestMe
    {
    }
}
");

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
        public void No_issue_is_reported_for_sealed_private_class_that_inherits_from_private_class() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_unsealed_private_class_that_inherits_from_private_class()
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

        [Test]
        public void Code_gets_fixed_for_unsealed_private_class_that_becomes_static()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private class Helper
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private static class Helper
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unsealed_private_class_that_becomes_sealed()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private class Unsealed
        {
            public Unsealed() { }
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        private sealed class Unsealed
        {
            public Unsealed() { }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unsealed_private_class_that_inherits_from_private_class()
        {
            const string OriginalCode = @"
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

            const string FixedCode = @"
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
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_unsealed_internal_partial_class()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public class Unsealed
        {
        }

        internal partial class Sealed : Unsealed
        {
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public class Unsealed
        {
        }

        internal sealed partial class Sealed : Unsealed
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3075_CodeFixProvider();
    }
}