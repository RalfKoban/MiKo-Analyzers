using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2024_EnumParamDefaultPhraseAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(object o) { }
}
");

        [TestCase("System.Boolean")]
        [TestCase("Boolean")]
        [TestCase("bool")]
        [TestCase(nameof(System.Boolean))]
        [TestCase(nameof(System.Object))]
        public void No_issue_is_reported_for_method_with_(string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(" + type + @" o) { }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_out_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>Whatever</param>
    public void DoSomething(out StringComparison o) { }
}
");

        [TestCase("One of the enumeration members that specifies something.")]
        [TestCase("One of the enumeration members that specifies something")]
        [TestCase("Unused")]
        [TestCase("Unused.")]
        public void No_issue_is_reported_for_method_with_correct_comment(string comment) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(StringComparison o) { }
}
");

        [TestCase("whatever.")]
        [TestCase("Whatever.")]
        public void An_issue_is_reported_for_method_with_wrong_comment_phrase(string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary />
    /// <param name='o'>" + comment + @"</param>
    public void DoSomething(StringComparison o) { }
}
");

        [TestCase("<summary />")]
        [TestCase("<inheritdoc />")]
        [TestCase("<exclude />")]
        public void No_issue_is_reported_for_method_with_missing_documentation(string xmlElement) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// " + xmlElement + @"
    public void DoSomething(StringComparison o) { }
}
");

        protected override string GetDiagnosticId() => MiKo_2024_EnumParamDefaultPhraseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2024_EnumParamDefaultPhraseAnalyzer();
    }
}