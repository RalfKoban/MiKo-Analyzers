using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2080_FieldSummaryDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// </summary>
    private string m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_enumerable_field_([Values("Contains the", "The")] string comment) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @" data for the field.
    /// </summary>
    private List<string> m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_boolean_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Indicates whether something is the data for the field.
    /// </summary>
    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_Guid_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The unique identifier for something that is the data for the field.
    /// </summary>
    private Guid m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_const_field() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// The field.
    /// </summary>
    public const string Field;
}
");

        [Test]
        public void No_issue_is_reported_for_enum_member() => No_issue_is_reported_for(@"
using System;

public enum TestMe
{
    /// <summary>
    /// Bla bla.
    /// </summary>
    Field = 0,
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_field_([Values("Bla bla.", "Contains something.", "Indicates whether something.")] string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private string m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_enumerable_field_([Values("Bla bla", "Indicates whether something.")] string comment) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private List<string> m_field;
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_boolean_field_([Values("Bla bla", "The field", "Contains something.")] string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private bool m_field;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_DependencyProperty_field_summary_with_readonly_comment() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public int MyDependency { get; set; }

    /// <summary>
    /// Identifies the <see cref=""MyDependency""/> dependency property. This field is read-only.
    /// </summary>
    private static readonly DependencyProperty MyDependencyProperty;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_RoutedEvent_field_summary_with_readonly_comment() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    /// <summary>
    /// Identifies the <see cref=""TouchUp""/> routed event.
    /// </summary>
    public static readonly System.Windows.RoutedEvent TouchUpEvent;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_constant_boolean_field() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// The Bla bla
    /// </summary>
    private const bool m_field;
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_commented_constant_boolean_field_(
                                                                                       [Values("Bla bla", "Indicates whether the field", "Contains something.")] string comment,
                                                                                       [Values("bool")] string fieldType)
            => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    private const " + fieldType + @" m_field;
}
");

        [Test]
        public void Code_gets_fixed_for_constant_boolean_field()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Some comment.
    /// </summary>
    private const bool m_field;
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// The some comment.
    /// </summary>
    private const bool m_field;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_constant_boolean_field()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Some comment.
    /// </summary>
    private bool m_field;
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Indicates whether some comment.
    /// </summary>
    private bool m_field;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Guid_field()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Some comment.
    /// </summary>
    private Guid m_field;
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// The unique identifier for some comment.
    /// </summary>
    private Guid m_field;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_collection_field()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Some comment.
    /// </summary>
    private List<string> m_field;
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Contains some comment.
    /// </summary>
    private List<string> m_field;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_normal_field()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// Some comment.
    /// </summary>
    private object m_field;
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    /// <summary>
    /// The some comment.
    /// </summary>
    private object m_field;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2080_FieldSummaryDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2080_FieldSummaryDefaultPhraseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2080_CodeFixProvider();
    }
}