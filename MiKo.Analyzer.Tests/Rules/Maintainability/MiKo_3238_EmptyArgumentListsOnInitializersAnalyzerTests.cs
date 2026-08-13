using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3238_EmptyArgumentListsOnInitializersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_object_initializer_without_parenthesis() => No_issue_is_reported_for(@"
using System;
using System.IO;

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
        public void No_issue_is_reported_for_object_initializer_with_parenthesis_containing_parameters() => No_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe(long Value)
    {
        public int Id { get; set; }

        public static TestMe Create() => new TestMe(4711)
                                             {
                                                 Id = 42,
                                             };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_collection_initializer_without_parenthesis() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int>
                                                {
                                                    1,
                                                    2,
                                                };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_collection_initializer_with_parenthesis_containing_parameters() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int>(42)
                                                {
                                                    1,
                                                    2,
                                                };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_implicit_object_initializer_with_parenthesis_containing_no_parameters() => No_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public int Id { get; set; }

        private static readonly TestMe = new()
                                             {
                                                 Id = 42,
                                             };
    }
}
");

        [Test]
        public void No_issue_is_reported_for_implicit_collection_initializer_with_parenthesis_containing_no_parameters() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        private static readonly List<int> list = new()
                                                     {
                                                         1,
                                                         2,
                                                     };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_object_initializer_with_parenthesis_containing_no_parameters() => An_issue_is_reported_for(@"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public int Id { get; set; }

        public static TestMe Create() => new TestMe()
                                             {
                                                 Id = 42,
                                             };
    }
}
");

        [Test]
        public void An_issue_is_reported_for_collection_initializer_with_parenthesis_containing_no_parameters() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int>()
                                                {
                                                    1,
                                                    2,
                                                };
    }
}
");

        [Test]
        public void Code_gets_fixed_for_object_initializer_with_parenthesis_containing_no_parameters()
        {
            const string OriginalCode = @"
using System;
using System.IO;

namespace Bla
{
    public class TestMe
    {
        public int Id { get; set; }

        public static TestMe Create() => new TestMe()
                                             {
                                                 Id = 42,
                                             };
    }
}
";

            const string FixedCode = @"
using System;
using System.IO;

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
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_collection_initializer_with_parenthesis_containing_no_parameters()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int>()
                                                {
                                                    1,
                                                    2,
                                                };
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public static List<int> Create() => new List<int>
                                                {
                                                    1,
                                                    2,
                                                };
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3238_EmptyArgumentListsOnInitializersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3238_EmptyArgumentListsOnInitializersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3238_CodeFixProvider();
    }
}