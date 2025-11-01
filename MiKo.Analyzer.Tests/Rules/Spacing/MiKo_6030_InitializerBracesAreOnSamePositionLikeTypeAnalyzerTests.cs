﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6030_InitializerBracesAreOnSamePositionLikeTypeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_field_with_reduced_array_initializer_when_placed_on_same_line() => No_issue_is_reported_for(@"
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
    private HashSet<int> MyField = new HashSet<int> { 1, 2, 3 };
}
");

        [Test]
        public void No_issue_is_reported_for_implicit_field_collection_initializer_when_placed_on_same_line() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private HashSet<int> MyField = new() { 1, 2, 3 };
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
        public void No_issue_is_reported_for_field_with_reduced_array_initializer_when_placed_on_same_position_as_type() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_field_with_implicit_array_initializer_when_placed_on_same_position_as_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int[] MyField = new[]
                                {
                                    1,
                                    2,
                                    3,
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
    private HashSet<int> MyField = new HashSet<int>
                                       {
                                           1,
                                           2,
                                           3,
                                       };
}
");

        [Test]
        public void No_issue_is_reported_for_implicit_field_collection_initializer_when_placed_on_same_position_as_type() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private HashSet<int> MyField = new()
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
        public void No_issue_is_reported_for_field_anonymous_object_initializer_when_placed_on_same_position_as_potential_type() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new
                                                   {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_field_with_reduced_array_initializer_when_placed_on_position_before_position_of_type() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_field_with_reduced_array_initializer_when_placed_on_position_after_position_of_type() => An_issue_is_reported_for(@"
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
    private HashSet<int> MyField = new HashSet<int>
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
    private HashSet<int> MyField = new HashSet<int>
                                        {
                                           1,
                                           2,
                                           3,
                                       };
}
");

        [Test]
        public void An_issue_is_reported_for_implicit_field_collection_initializer_when_placed_on_position_before_position_of_type() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private HashSet<int> MyField = new()
                                      {
                                           1,
                                           2,
                                           3,
                                       };
}
");

        [Test]
        public void An_issue_is_reported_for_implicit_field_collection_initializer_when_placed_on_position_after_position_of_type() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private HashSet<int> MyField = new()
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
        public void An_issue_is_reported_for_field_anonymous_object_initializer_when_placed_on_position_before_position_of_type() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new
                                                  {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_field_anonymous_object_initializer_when_placed_on_position_after_position_of_type() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new
                                                    {
                                                       Property1 = 1,
                                                       Property2 = 2,
                                                   };

    public int Property1 { get; set; }

    public int Property2 { get; set; }
}
");

        [Test]
        public void Code_gets_fixed_for_field_with_reduced_array_initializer_when_placed_on_position_before_position_of_type()
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
        public void Code_gets_fixed_for_field_with_reduced_array_initializer_when_placed_on_position_after_position_of_type()
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
        public void Code_gets_fixed_for_field_with_implicit_array_initializer_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = new[]
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
    private int[] MyField = new[]
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
        public void Code_gets_fixed_for_field_with_implicit_array_initializer_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = new[]
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
    private int[] MyField = new[]
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
    private HashSet<int> MyField = new HashSet<int>
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
    private HashSet<int> MyField = new HashSet<int>
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
    private HashSet<int> MyField = new HashSet<int>
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
    private HashSet<int> MyField = new HashSet<int>
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
        public void Code_gets_fixed_for_field_collection_initializer_when_some_values_are_on_same_lines_and_initializer_is_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private HashSet<int> MyField = new HashSet<int>
                                        {
                                            1, 2, 3,
                                            4, 5, 6,
                                       };
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private HashSet<int> MyField = new HashSet<int>
                                       {
                                           1, 2, 3,
                                           4, 5, 6,
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
        public void Code_gets_fixed_for_field_anonymous_object_initializer_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new
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
    private static readonly TestMe _instance = new
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
        public void Code_gets_fixed_for_field_anonymous_object_initializer_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private static readonly TestMe _instance = new
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
    private static readonly TestMe _instance = new
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

        [Test]
        public void Code_gets_fixed_for_complete_field_array_initializer_when_some_values_are_on_same_lines_and_initializer_is_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                            {
                                1, 2, 3,
                                4, 5, 6,
                            };
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    private int[] MyField = new int[]
                                {
                                    1, 2, 3,
                                    4, 5, 6,
                                };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_assignment_of_collection_initializer()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<int> MyProperty { get; set; }

    public TestMe Create()
    {
        var result = new TestMe
                         {
                            MyProperty =
                                    {
                                        1, 2, 3,
                                        4, 5, 6,
                                    },
                         };

        return result;
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public List<int> MyProperty { get; set; }

    public TestMe Create()
    {
        var result = new TestMe
                         {
                            MyProperty =
                                         {
                                             1, 2, 3,
                                             4, 5, 6,
                                         },
                         };

        return result;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_array_initializer_with_contained_implicit_object_creations()
        {
            const string OriginalCode = @"
using System;

public class Dto
{
    public int Id { get; set; }
    public int Value { get; set; }
}

public class TestMe
{
    private Dto[] MyField = new Dto[]
                            {
                              new() { Id = -1, Value = 4711 },
                              new()
                            {
                                Id = 1,
                                Value = 42,
                            },
                              new() { Id = 2, Value = 0815 },
                            };
}
";

            const string FixedCode = @"
using System;

public class Dto
{
    public int Id { get; set; }
    public int Value { get; set; }
}

public class TestMe
{
    private Dto[] MyField = new Dto[]
                                {
                                    new() { Id = -1, Value = 4711 },
                                    new()
                                        {
                                            Id = 1,
                                            Value = 42,
                                        },
                                    new() { Id = 2, Value = 0815 },
                                };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_array_initializer_with_contained_anonymous_object_creations()
        {
            const string OriginalCode = @"
using System;

public class Dto
{
    public int Id { get; set; }
    public int Value { get; set; }
}

public class TestMe
{
    private Dto[] MyField = new Dto[]
                            {
                              new { Id = -1, Value = 4711 },
                              new
                            {
                                Id = 1,
                                Value = 42,
                            },
                              new { Id = 2, Value = 0815 },
                            };
}
";

            const string FixedCode = @"
using System;

public class Dto
{
    public int Id { get; set; }
    public int Value { get; set; }
}

public class TestMe
{
    private Dto[] MyField = new Dto[]
                                {
                                    new { Id = -1, Value = 4711 },
                                    new
                                        {
                                            Id = 1,
                                            Value = 42,
                                        },
                                    new { Id = 2, Value = 0815 },
                                };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_field_array_initializer_with_contained_object_creations()
        {
            const string OriginalCode = @"
using System;

public class Dto
{
    public int Id { get; set; }
    public int Value { get; set; }
}

public class TestMe
{
    private Dto[] MyField = new Dto[]
                            {
                              new Dto { Id = -1, Value = 4711 },
                              new Dto
                            {
                                Id = 1,
                                Value = 42,
                            },
                              new Dto { Id = 2, Value = 0815 },
                            };
}
";

            const string FixedCode = @"
using System;

public class Dto
{
    public int Id { get; set; }
    public int Value { get; set; }
}

public class TestMe
{
    private Dto[] MyField = new Dto[]
                                {
                                    new Dto { Id = -1, Value = 4711 },
                                    new Dto
                                        {
                                            Id = 1,
                                            Value = 42,
                                        },
                                    new Dto { Id = 2, Value = 0815 },
                                };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_implicit_field_collection_initializer_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private HashSet<int> MyField = new()
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
    private HashSet<int> MyField = new()
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
        public void Code_gets_fixed_for_implicit_field_collection_initializer_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    private HashSet<int> MyField = new()
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
    private HashSet<int> MyField = new()
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
        public void Code_gets_fixed_for_implicit_field_object_initializer_with_arguments_when_placed_on_position_before_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe : IList<int>
{
    public TestMe(int id)
    {
    }

    private TestMe MyField = new(42)
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

public class TestMe : IList<int>
{
    public TestMe(int id)
    {
    }

    private TestMe MyField = new(42)
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
        public void Code_gets_fixed_for_implicit_field_object_initializer_with_arguments_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe : IList<int>
{
    public TestMe(int id)
    {
    }

    private TestMe MyField = new(42)
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

public class TestMe : IList<int>
{
    public TestMe(int id)
    {
    }

    private TestMe MyField = new(42)
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
        public void Code_gets_fixed_for_nested_dictionary_initializer_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public record Testee
{
    public Dictionary<string, string[]> Headers { get; set; }
}

public class TestMe
{
    private Testee DoSomething()
    {
        var result = new Testee
        {
            Headers = new Dictionary(string, string[])
            {
                { ""key"", [""value""] },
            },
        };

        return result;
    }
}
";
            const string FixedCode = @"
using System;
using System.Collections.Generic;

public record Testee
{
    public Dictionary<string, string[]> Headers { get; set; }
}

public class TestMe
{
    private Testee DoSomething()
    {
        var result = new Testee
                         {
                             Headers = new Dictionary(string, string[])
                                           {
                                               { ""key"", [""value""] },
                                           },
                         };

        return result;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_dictionary_initializer_spanning_multiple_lines_when_placed_on_position_after_position_of_type()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public record Testee
{
    public Dictionary<string, string[]> Headers { get; set; }
}

public class TestMe
{
    private Testee DoSomething()
    {
        var result = new Testee
        {
            Headers = new Dictionary(string, string[])
            {
                {
                    ""key"", [""value""]
                },
            },
        };

        return result;
    }
}
";
            const string FixedCode = @"
using System;
using System.Collections.Generic;

public record Testee
{
    public Dictionary<string, string[]> Headers { get; set; }
}

public class TestMe
{
    private Testee DoSomething()
    {
        var result = new Testee
                         {
                             Headers = new Dictionary(string, string[])
                                           {
                                               {
                                                 ""key"", [""value""]
                                               },
                                           },
                         };

        return result;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_object_initializer_spanning_multiple_lines()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public record Testee
{
    public Testee Nested { get; set; }

    public string Name { get; set; }
}

public class TestMe
{
    private Testee DoSomething()
    {
        var result = new Testee
        {
            Nested = new Testeee
            {
                Name = ""nested""
            },
            Name = ""unnested""
        };

        return result;
    }
}
";
            const string FixedCode = @"
using System;
using System.Collections.Generic;

public record Testee
{
    public Testee Nested { get; set; }

    public string Name { get; set; }
}

public class TestMe
{
    private Testee DoSomething()
    {
        var result = new Testee
                         {
                             Nested = new Testeee
                                          {
                                              Name = ""nested""
                                          },
                             Name = ""unnested""
                         };

        return result;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_implicit_field_object_initializer_with_arguments_when_placed_outdented_below_type_and_space_between_braces()
        {
            const string OriginalCode = @"
using System;

public record Item
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Information { get; set; }
}

public class TestMe
{
    private readonly Item _item = new()
        { Name = ""test name"", FullName = ""complete test name"", Information = ""some information"" };
}
";

            const string FixedCode = @"
using System;

public record Item
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Information { get; set; }
}

public class TestMe
{
    private readonly Item _item = new()
                                      { Name = ""test name"", FullName = ""complete test name"", Information = ""some information"" };
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_implicit_field_object_initializer_with_arguments_when_placed_outdented_below_type_and_no_space_between_braces()
        {
            const string OriginalCode = @"
using System;

public record Item
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Information { get; set; }
}

public class TestMe
{
    private readonly Item _item = new()
        {Name = ""test name"", FullName = ""complete test name"", Information = ""some information""};
}
";

            const string FixedCode = @"
using System;

public record Item
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Information { get; set; }
}

public class TestMe
{
    private readonly Item _item = new()
                                      {Name = ""test name"", FullName = ""complete test name"", Information = ""some information""};
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_list_of_dictionary_with_implicit_dictionary_creation()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        var result = new List<Dictionary<string, object>>
        {
            new() {
                { ""some"", ""value"" },
                { ""another"", ""item"" },
                { ""third"", ""entry"" }
            }
        };
    }
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        var result = new List<Dictionary<string, object>>
                         {
                             new()
                                 {
                                     { ""some"", ""value"" },
                                     { ""another"", ""item"" },
                                     { ""third"", ""entry"" }
                                 }
                         };
    }
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_list_of_dictionary_with_explicit_dictionary_creation()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        var result = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>() {
                { ""some"", ""value"" },
                { ""another"", ""item"" },
                { ""third"", ""entry"" }
            }
        };
    }
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        var result = new List<Dictionary<string, object>>
                         {
                             new Dictionary<string, object>()
                                 {
                                     { ""some"", ""value"" },
                                     { ""another"", ""item"" },
                                     { ""third"", ""entry"" }
                                 }
                         };
    }
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_list_of_dictionary_with_explicit_dictionary_creation_and_element_expressions_are_on_separate_lines()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        var result = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>() {
                {
                    ""some"", ""value"" },
                {
                    ""another"", ""item"" },
                {
                    ""third"", ""entry"" }
            }
        };
    }
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething()
    {
        var result = new List<Dictionary<string, object>>
                         {
                             new Dictionary<string, object>()
                                 {
                                     {
                                       ""some"", ""value"" },
                                     {
                                       ""another"", ""item"" },
                                     {
                                       ""third"", ""entry"" }
                                 }
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