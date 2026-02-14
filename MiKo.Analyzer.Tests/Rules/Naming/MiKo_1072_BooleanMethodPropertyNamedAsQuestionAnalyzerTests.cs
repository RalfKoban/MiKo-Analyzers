using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
                                                        [
                                                            "AreConnected",
                                                            "CanBeConnected",
                                                            "Connected",
                                                            "HasConnectionEstablished",
                                                            "IsAllLowerCase",
                                                            "IsAllUpperCase",
                                                            "IsAnyKind",
                                                            "IsAssignableFrom",
                                                            "IsAssignableTo",
                                                            "IsByteArray",
                                                            "IsCancellationToken",
                                                            "IsCompleted",
                                                            "IsConnected",
                                                            "IsContainedInData",
                                                            "IsDefault",
                                                            "IsDefaultValue",
                                                            "IsDigitsOnly",
                                                            "IsDragSource",
                                                            "IsDroppedOverSomething",
                                                            "IsDropTarget",
                                                            "IsEmptyArray",
                                                            "IsExcludedFromData",
                                                            "IsFirstChild",
                                                            "IsIncludedInData",
                                                            "IsInCode",
                                                            "IsInDesign",
                                                            "IsInDesignerMode",
                                                            "IsInDesignMode",
                                                            "IsInheritedFromSomething",
                                                            "IsInsideOfSomething",
                                                            "IsLastChild",
                                                            "IsLowerCase",
                                                            "IsLowerCaseLetter",
                                                            "IsNameOf",
                                                            "IsNavigationTarget",
                                                            "IsNextChild",
                                                            "IsNotCompleted",
                                                            "IsOfName",
                                                            "IsOfType",
                                                            "IsOfTypeWhatever",
                                                            "IsOnSameLineLine",
                                                            "IsPreviousChild",
                                                            "IsPrimaryConstructor",
                                                            "IsReadOnly",
                                                            "IsReadWrite",
                                                            "IsSame",
                                                            "IsSameKey",
                                                            "IsShownAs",
                                                            "IsShownAsText",
                                                            "IsShownIn",
                                                            "IsShownInMenu",
                                                            "IsSingleWord",
                                                            "IsSolutionWide",
                                                            "IsTest",
                                                            "IsTestMethod",
                                                            "IsTypeOf",
                                                            "IsUpperCase",
                                                            "IsUpperCaseLetter",
                                                            "IsValueConverter",
                                                            "IsWhiteSpace",
                                                            "IsWhiteSpaceOnly",
                                                            "IsWriteProtected",
                                                            "IsZipFile",
                                                            nameof(string.IsNullOrEmpty),
                                                            nameof(string.IsNullOrWhiteSpace),
                                                        ];

        private static readonly string[] WrongNames =
                                                      [
                                                          "IsConnectionPossible",
                                                          "AreDevicesConnected",
                                                      ];

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_boolean_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void IsDoingSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_boolean_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int IsDoingSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_test_method_([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

[TestFixture]
public class TestMe
{
    [Test]
    public bool " + name + @"()
    {
        return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_method_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @"()
    {
        return false;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_property_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @" { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_boolean_method_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @"()
    {
        return false;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_boolean_property_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @" { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1072_BooleanMethodPropertyNamedAsQuestionAnalyzer();
    }
}