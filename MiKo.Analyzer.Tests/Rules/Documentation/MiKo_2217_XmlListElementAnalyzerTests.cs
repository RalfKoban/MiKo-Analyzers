using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2217_XmlListElementAnalyzerTests : CodeFixVerifier
    {
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
        public void No_issue_is_reported_for_documented_method_with_list_without_type() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_documented_method_with_list_of_type_bullet_with_items_and_description_only() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""bullet"">
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
        public void No_issue_is_reported_for_documented_method_with_list_of_type_table_with_listheader_and_term_and_description() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_documented_method_with_list_of_unknown_type() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_documented_method_with_list_of_type_bullet_with_listheader() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""bullet"">
    ///   <listheader>
    ///   </listheader>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_documented_method_with_list_of_type_number_with_listheader() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""number"">
    ///   <listheader>
    ///   </listheader>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_documented_method_with_list_of_type_number_with_multiple_terms_in_item() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""number"">
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
        public void An_issue_is_reported_for_documented_method_with_list_of_type_number_with_multiple_descriptions_in_item() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>Does something.</summary>
    /// <remarks>
    /// <list type=""number"">
    ///   <item>
    ///     <description>Some text</description>
    ///     <description>Some more text</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_documented_method_with_list_of_type_table_with_listheader_but_no_term_or_description() => An_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_documented_method_with_list_of_type_table_with_listheader_and_multiple_terms_and_multiple_descriptions_mixed_up_in_different_items() => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_2217_XmlListElementAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2217_XmlListElementAnalyzer();
    }
}