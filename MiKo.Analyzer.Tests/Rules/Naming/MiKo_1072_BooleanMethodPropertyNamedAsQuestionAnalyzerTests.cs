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
        public void No_issue_is_reported_for_method_with_non_Boolean_return_type() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_property_with_non_Boolean_return_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public int IsDoingSomething { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_Boolean_return_type_([ValueSource(nameof(WrongNames))] string name) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_method_with_Boolean_return_type_and_correct_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_property_with_Boolean_return_type_and_correct_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public bool " + name + @" { get; set; }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Boolean_return_type_and_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_property_with_Boolean_return_type_and_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
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