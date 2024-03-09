using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3049_EnumMemberHasDescriptionAttributeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_enum_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }

    private readonly int m_id = 1;
}
");

        [Test]
        public void No_issue_is_reported_for_empty_enum_type() => No_issue_is_reported_for(@"
using System;

public enum TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_enum_type_in_NativeDeclarations_type() => No_issue_is_reported_for(@"
using System;

public class NativeDeclarations
{
    public enum TestMe
    {
        None = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_enum_type_in_Interop_namespace() => No_issue_is_reported_for(@"
using System;

namespace Something.Interop
{
    public enum TestMe
    {
        None = 0;
    }
}
");

        [Test, Ignore("Roslyn attribute issue: Detection of attribute's ctor currently does not work within test, but works in production code.")]
        public void No_issue_is_reported_for_documented_enum_type_with_description() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public enum TestMe
{
    [Description(""some description"")]
    None = 0,
}
");

        [Test]
        public void An_issue_is_reported_for_documented_enum_type_without_description() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public enum TestMe
{
    [Description]
    None = 0,
}
");

        [Test]
        public void An_issue_is_reported_for_partly_documented_enum_type() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public enum TestMe
{
    [Description(""some description"")]
    None = 0,
    Something = 1,
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_undocumented_enum_type() => An_issue_is_reported_for(2, @"
using System;
using System.ComponentModel;

public enum TestMe
{
    None = 0,
    Something = 1,
}
");

        protected override string GetDiagnosticId() => MiKo_3049_EnumMemberHasDescriptionAttributeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3049_EnumMemberHasDescriptionAttributeAnalyzer();
    }
}