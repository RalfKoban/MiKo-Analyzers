using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2217_XmlListElementAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] EnumeratedListTypes = { "bullet", "number" };

        [Test]
        public void No_issue_is_reported_for_undocumented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_documented_method_without_list() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>Does something.</remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_list_without_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_list_containing_items_and_description_only_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""" + type + @""">
    ///   <item>
    ///     <description>Some text</description>
    ///   </item>
    ///   <item>
    ///     <description>More text</description>
    ///   </item>
    ///   <item>
    ///     <description>Even more text</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_list_of_type_table_with_listheader_and_term_and_description() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""table"">
    ///   <listheader>
    ///     <term>Some text</term>
    ///     <description>Some description</description>
    ///   </listheader>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_list_with_unknown_type() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""some unknown type"">
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_list_containing_listheader_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""" + type + @""">
    ///   <listheader>
    ///   </listheader>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_list_containing_listheader_but_no_term_or_description_for_type_table() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""table"">
    ///   <listheader>
    ///   </listheader>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_list_containing_multiple_terms_in_item_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type) => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""" + type + @""">
    ///   <item>
    ///     <term>Some text</term>
    ///     <term>Some more text</term>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_list_containing_multiple_descriptions_in_item_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""" + type + @""">
    ///   <item>
    ///     <description>Some text</description>
    ///     <description>Some more text</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_list_containing_multiple_items_that_contain_only_text_but_no_description_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type) => An_issue_is_reported_for(2, @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""" + type + @""">
    ///   <item>Some text</item>
    ///   <item>Some more text</item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_list_of_type_table_with_listheader_and_multiple_terms_and_multiple_descriptions_mixed_up_in_different_items() => An_issue_is_reported_for(3, @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""table"">
    ///   <listheader>
    ///     <term>Some text</term>
    ///     <description>Some description</description>
    ///   </listheader>
    ///   <item>
    ///     <term>Some text</term>
    ///     <term>Some more text</term>
    ///   </item>
    ///   <item>
    ///     <description>Some description</description>
    ///     <description>Some more description</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void Code_gets_fixed_for_list_containing_listheader_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type)
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""###"">
    ///   <listheader>
    ///   </listheader>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";
            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""###"">
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_list_containing_multiple_terms_in_item_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type)
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""###"">
    ///   <item>
    ///     <term>Some text</term>
    ///     <term>Some more text</term>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""###"">
    ///   <item><description>Some text</description></item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_list_containing_multiple_descriptions_in_item_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type)
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""###"">
    ///   <item>
    ///     <description>Some text</description>
    ///     <description>Some more text</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""###"">
    ///   <item>
    ///     <description>Some text</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_list_containing_multiple_items_that_contain_only_text_but_no_description_for_type_([ValueSource(nameof(EnumeratedListTypes))] string type)
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""###"">
    ///   <item>Some text</item>
    ///   <item>Some more text</item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""###"">
    ///   <item><description>Some text</description></item>
    ///   <item><description>Some more text</description></item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", type), FixedCode.Replace("###", type));
        }

        [Test]
        public void Code_gets_fixed_for_list_of_type_table_with_listheader_and_2_terms_and_2_descriptions_mixed_up_in_different_items()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""table"">
    ///   <listheader>
    ///     <term>Some text</term>
    ///     <description>Some description</description>
    ///   </listheader>
    ///   <item>
    ///     <term>Some text</term>
    ///     <term>Some more text</term>
    ///   </item>
    ///   <item>
    ///     <description>Some description</description>
    ///     <description>Some more description</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""table"">
    ///   <listheader>
    ///     <term>Some text</term>
    ///     <description>Some description</description>
    ///   </listheader>
    ///   <item>
    ///     <term>Some text</term>
    ///     <description>Some more text</description>
    ///   </item>
    ///   <item>
    ///     <term>Some description</term>
    ///     <description>Some more description</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_list_of_type_table_with_listheader_and_3_terms_and_3_descriptions_mixed_up_in_different_items()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""table"">
    ///   <listheader>
    ///     <term>Some text</term>
    ///     <description>Some description</description>
    ///     <description>Some additional description</description>
    ///   </listheader>
    ///   <item>
    ///     <term>Some text</term>
    ///     <term>Some more text</term>
    ///     <term>Some even more text</term>
    ///   </item>
    ///   <item>
    ///     <description>Some description</description>
    ///     <description>Some more description</description>
    ///     <description>Some even more description</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""table"">
    ///   <listheader>
    ///     <term>Some text</term>
    ///     <term>Some description</term>
    ///     <term>Some additional description</term>
    ///   </listheader>
    ///   <item>
    ///     <term>Some text</term>
    ///     <term>Some more text</term>
    ///     <term>Some even more text</term>
    ///   </item>
    ///   <item>
    ///     <term>Some description</term>
    ///     <term>Some more description</term>
    ///     <term>Some even more description</term>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2217_XmlListElementAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2217_XmlListElementAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2217_CodeFixProvider();
    }
}