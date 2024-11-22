﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3121_ObjectUnderTestIsNoInterfaceAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AcceptableTestTypes = ["class", "struct", "record"];

        private static readonly string[] PropertyNames =
                                                         [
                                                             "ObjectUnderTest",
                                                             "SubjectUnderTest",
                                                             "UnitUnderTest",
                                                             "Sut",
                                                             "SuT",
                                                             "SUT",
                                                             "UUT",
                                                             "UuT",
                                                             "Uut",
                                                             "TestCandidate",
                                                             "TestObject",
                                                         ];

        private static readonly string[] FieldNames =
                                                      [
                                                          "ObjectUnderTest",
                                                          "_ObjectUnderTest",
                                                          "m_ObjectUnderTest",
                                                          "s_ObjectUnderTest",
                                                          "objectUnderTest",
                                                          "_objectUnderTest",
                                                          "m_objectUnderTest",
                                                          "s_objectUnderTest",
                                                          "subjectUnderTest",
                                                          "_subjectUnderTest",
                                                          "m_subjectUnderTest",
                                                          "s_subjectUnderTest",
                                                          "SubjectUnderTest",
                                                          "_SubjectUnderTest",
                                                          "m_SubjectUnderTest",
                                                          "s_SubjectUnderTest",
                                                          "unitUnderTest",
                                                          "_unitUnderTest",
                                                          "m_unitUnderTest",
                                                          "s_unitUnderTest",
                                                          "UnitUnderTest",
                                                          "_UnitUnderTest",
                                                          "m_UnitUnderTest",
                                                          "s_UnitUnderTest",
                                                          "sut",
                                                          "_sut",
                                                          "m_sut",
                                                          "s_sut",
                                                          "Sut",
                                                          "_Sut",
                                                          "m_Sut",
                                                          "s_Sut",
                                                          "uut",
                                                          "_uut",
                                                          "m_uut",
                                                          "s_uut",
                                                          "Uut",
                                                          "_Uut",
                                                          "m_Uut",
                                                          "s_Uut",
                                                          "TestCandidate",
                                                          "testCandidate",
                                                          "_testCandidate",
                                                          "m_testCandidate",
                                                          "s_testCandidate",
                                                          "TestObject",
                                                          "testObject",
                                                          "_testObject",
                                                          "m_testObject",
                                                          "s_testObject",
                                                      ];

        private static readonly string[] VariableNames =
                                                         [
                                                             "objectUnderTest",
                                                             "subjectUnderTest",
                                                             "unitUnderTest",
                                                             "testCandidate",
                                                             "testObject",
                                                             "sut",
                                                             "uut",
                                                         ];

        private static readonly string[] MethodPrefixes =
                                                          [
                                                              "Get",
                                                              "Create",
                                                          ];

        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
namespace BlaBla
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_class_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
namespace BlaBla
{
    [" + fixture + @"]
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_if_type_under_test_is_(
                                                                         [ValueSource(nameof(AcceptableTestTypes))] string type,
                                                                         [ValueSource(nameof(TestFixtures))] string fixture,
                                                                         [ValueSource(nameof(PropertyNames))] string propertyName)
            => No_issue_is_reported_for(@"
namespace BlaBla
{
    public " + type + @" TestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + propertyName + @" { get; set; }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_if_type_under_test_is_(
                                                                       [ValueSource(nameof(AcceptableTestTypes))] string type,
                                                                       [ValueSource(nameof(TestFixtures))] string fixture,
                                                                       [ValueSource(nameof(PropertyNames))] string propertyName,
                                                                       [ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => No_issue_is_reported_for(@"
namespace BlaBla
{
    public " + type + @" TestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + methodPrefix + propertyName + @"() => null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_if_type_under_test_is_(
                                                                      [ValueSource(nameof(AcceptableTestTypes))] string type,
                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                      [ValueSource(nameof(FieldNames))] string fieldName)
            => No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public " + type + @" TestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private TestMe " + fieldName + @";
    }
}
");

        [Test]
        public void No_issue_is_reported_for_localVariable_if_type_under_test_is_(
                                                                              [ValueSource(nameof(AcceptableTestTypes))] string type,
                                                                              [ValueSource(nameof(TestFixtures))] string fixture,
                                                                              [ValueSource(nameof(Tests))] string test,
                                                                              [ValueSource(nameof(VariableNames))] string variableName)
            => No_issue_is_reported_for(@"
namespace BlaBla
{
    public " + type + @" TestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        [" + test + @"]
        public void DoSomething()
        {
            var " + variableName + @" = new TestMe();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_if_type_under_test_is_an_interface_(
                                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                      [ValueSource(nameof(PropertyNames))] string propertyName)
            => An_issue_is_reported_for(@"
namespace BlaBla
{
    public interface ITestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private ITestMe " + propertyName + @" { get; set; }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_if_type_under_test_is_an_interface_(
                                                                                    [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                    [ValueSource(nameof(PropertyNames))] string propertyName,
                                                                                    [ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => An_issue_is_reported_for(@"
namespace BlaBla
{
    public interface ITestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private ITestMe " + methodPrefix + propertyName + @"() => null;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_field_if_type_under_test_is_an_interface_(
                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                   [ValueSource(nameof(FieldNames))] string fieldName)
            => An_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public interface ITestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        private ITestMe " + fieldName + @";
    }
}
");

        [Test]
        public void An_issue_is_reported_for_localVariable_if_type_under_test_is_an_interface_(
                                                                                           [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                           [ValueSource(nameof(Tests))] string test,
                                                                                           [ValueSource(nameof(VariableNames))] string variableName)
            => An_issue_is_reported_for(@"
namespace BlaBla
{
    public interface ITestMe
    {
    }

    public class TestMe : ITestMe
    {
    }

    [" + fixture + @"]
    public class TestMeTests
    {
        [" + test + @"]
        public void DoSomething()
        {
            ITestMe " + variableName + @" = new TestMe();
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3121_ObjectUnderTestIsNoInterfaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3121_ObjectUnderTestIsNoInterfaceAnalyzer();
    }
}