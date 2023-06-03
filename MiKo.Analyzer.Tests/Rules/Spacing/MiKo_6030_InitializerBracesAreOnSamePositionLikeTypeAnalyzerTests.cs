using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_field_reduced_array_initializer_when_placed_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = { 1, 2, 3 };
}
");

        [Test]
        public void No_issue_is_reported_for_field_array_initializer_when_placed_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = new int[] { 1, 2, 3 };
}
");

        [Test]
        public void No_issue_is_reported_for_field_collection_initializer_when_placed_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private new HashSet<int> MyField = new HashSet<int> { 1, 2, 3 };
}
");

        [Test]
        public void No_issue_is_reported_for_field_object_initializer_when_placed_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new TestMe { Property1 = 1, Property2 = 2 };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_field_reduced_array_initializer_when_placed_on_same_position_as_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField =
                            {
                                1,
                                2,
                                3
                            };
}
");

        [Test]
        public void No_issue_is_reported_for_field_array_initializer_when_placed_on_same_position_as_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                                {
                                    1,
                                    2,
                                    3,
                                };
}
");

        [Test]
        public void No_issue_is_reported_for_field_collection_initializer_when_placed_on_same_position_as_type() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private new HashSet<int> MyField = new HashSet<int>
                                           {
                                               1,
                                               2,
                                               3,
                                           };
}
");

        [Test]
        public void No_issue_is_reported_for_field_object_initializer_when_placed_on_same_position_as_type() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new TestMe
                                                   {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_field_reduced_array_initializer_when_placed_on_position_before_position_of_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField =
                          {
                                1,
                                2,
                                3
                            };
}
");

        [Test]
        public void An_issue_is_reported_for_field_reduced_array_initializer_when_placed_on_position_after_position_of_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField =
                             {
                                1,
                                2,
                                3
                            };
}
");

        [Test]
        public void An_issue_is_reported_for_field_array_initializer_when_placed_on_position_before_position_of_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                               {
                                    1,
                                    2,
                                    3,
                                };
}
");

        [Test]
        public void An_issue_is_reported_for_field_array_initializer_when_placed_on_position_after_position_of_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                                 {
                                    1,
                                    2,
                                    3,
                                };
}
");

        [Test]
        public void An_issue_is_reported_for_field_collection_initializer_when_placed_on_position_before_position_of_type() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private new HashSet<int> MyField = new HashSet<int>
                                          {
                                               1,
                                               2,
                                               3,
                                           };
}
");

        [Test]
        public void An_issue_is_reported_for_field_collection_initializer_when_placed_on_position_after_position_of_type() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private new HashSet<int> MyField = new HashSet<int>
                                            {
                                               1,
                                               2,
                                               3,
                                           };
}
");

        [Test]
        public void An_issue_is_reported_for_field_object_initializer_when_placed_on_position_before_position_of_type() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new TestMe
                                                  {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_field_object_initializer_when_placed_on_position_after_position_of_type() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new TestMe
                                                    {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
");

        [Test]
        public void Code_gets_fixed_for_field_reduced_array_initializer_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                          {
                                1,
                                2,
                                3
                            };
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                            {
                                1,
                                2,
                                3
                            };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_reduced_array_initializer_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                             {
                                1,
                                2,
                                3
                            };
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField =
                            {
                                1,
                                2,
                                3
                            };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_array_initializer_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                               {
                                    1,
                                    2,
                                    3,
                                };
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                                {
                                    1,
                                    2,
                                    3,
                                };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_array_initializer_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                                 {
                                    1,
                                    2,
                                    3,
                                };
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                                {
                                    1,
                                    2,
                                    3,
                                };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_collection_initializer_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private new HashSet<int> MyField = new HashSet<int>
                                          {
                                               1,
                                               2,
                                               3,
                                           };
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private new HashSet<int> MyField = new HashSet<int>
                                           {
                                               1,
                                               2,
                                               3,
                                           };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_collection_initializer_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private new HashSet<int> MyField = new HashSet<int>
                                            {
                                               1,
                                               2,
                                               3,
                                           };
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private new HashSet<int> MyField = new HashSet<int>
                                           {
                                               1,
                                               2,
                                               3,
                                           };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_object_initializer_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new TestMe
                                                  {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new TestMe
                                                   {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_object_initializer_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new TestMe
                                                    {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new TestMe
                                                   {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_complete_field_array_initializer_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = new int[]
    {
        1,
        2,
        3,
    };
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                                {
                                    1,
                                    2,
                                    3,
                                };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6030_CodeFixProvider();
    }
}