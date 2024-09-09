using Microsoft.CodeAnalysis.Diagnostics;

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
        public void No_issue_is_reported_for_empty_test_class() => Assert.Multiple(() =>
                                                                                        {
                                                                                            foreach (var testFixture in TestFixtures)
                                                                                            {
                                                                                                No_issue_is_reported_for(@"
namespace BlaBla
{
    [" + testFixture + @"]
    public class TestMe
    {
    }
}
");
                                                                                            }
                                                                                        });

        [Test]
        public void No_issue_is_reported_for_property_if_type_under_test_is_([ValueSource(nameof(AcceptableTestTypes))] string type) => Assert.Multiple(() =>
                                                                                                                                                             {
                                                                                                                                                                 foreach (var testFixture in TestFixtures)
                                                                                                                                                                 {
                                                                                                                                                                     foreach (var propertyName in PropertyNames)
                                                                                                                                                                     {
                                                                                                                                                                         No_issue_is_reported_for(@"
namespace BlaBla
{
    public " + type + @" TestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe " + propertyName + @" { get; set; }
    }
}
");
                                                                                                                                                                     }
                                                                                                                                                                 }
                                                                                                                                                             });

        [Test]
        public void No_issue_is_reported_for_method_if_type_under_test_is_([ValueSource(nameof(AcceptableTestTypes))] string type) => Assert.Multiple(() =>
                                                                                                                                                           {
                                                                                                                                                               foreach (var testFixture in TestFixtures)
                                                                                                                                                               {
                                                                                                                                                                   foreach (var propertyName in PropertyNames)
                                                                                                                                                                   {
                                                                                                                                                                       foreach (var methodPrefix in MethodPrefixes)
                                                                                                                                                                       {
                                                                                                                                                                           No_issue_is_reported_for(@"
namespace BlaBla
{
    public " + type + @" TestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe " + methodPrefix + propertyName + @"() => null;
    }
}
");
                                                                                                                                                                       }
                                                                                                                                                                   }
                                                                                                                                                               }
                                                                                                                                                           });

        [Test]
        public void No_issue_is_reported_for_field_if_type_under_test_is_([ValueSource(nameof(AcceptableTestTypes))] string type) => Assert.Multiple(() =>
                                                                                                                                                          {
                                                                                                                                                              foreach (var testFixture in TestFixtures)
                                                                                                                                                              {
                                                                                                                                                                  foreach (var fieldName in FieldNames)
                                                                                                                                                                  {
                                                                                                                                                                      No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public " + type + @" TestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe " + fieldName + @";
    }
}
");
                                                                                                                                                                  }
                                                                                                                                                              }
                                                                                                                                                          });

        [Test]
        public void No_issue_is_reported_for_localVariable_if_type_under_test_is_([ValueSource(nameof(AcceptableTestTypes))] string type) => Assert.Multiple(() =>
                                                                                                                                                                  {
                                                                                                                                                                      foreach (var testFixture in TestFixtures)
                                                                                                                                                                      {
                                                                                                                                                                          foreach (var test in Tests)
                                                                                                                                                                          {
                                                                                                                                                                              foreach (var variableName in VariableNames)
                                                                                                                                                                              {
                                                                                                                                                                                  No_issue_is_reported_for(@"
namespace BlaBla
{
    public " + type + @" TestMe
    {
    }

    [" + testFixture + @"]
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
                                                                                                                                                                              }
                                                                                                                                                                          }
                                                                                                                                                                      }
                                                                                                                                                                  });

        [Test]
        public void An_issue_is_reported_for_property_if_type_under_test_is_an_interface() => Assert.Multiple(() =>
                                                                                                                   {
                                                                                                                       foreach (var testFixture in TestFixtures)
                                                                                                                       {
                                                                                                                           foreach (var propertyName in PropertyNames)
                                                                                                                           {
                                                                                                                               An_issue_is_reported_for(@"
namespace BlaBla
{
    public interface ITestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private ITestMe " + propertyName + @" { get; set; }
    }
}
");
                                                                                                                           }
                                                                                                                       }
                                                                                                                   });

        [Test]
        public void An_issue_is_reported_for_method_if_type_under_test_is_an_interface() => Assert.Multiple(() =>
                                                                                                                 {
                                                                                                                     foreach (var testFixture in TestFixtures)
                                                                                                                     {
                                                                                                                         foreach (var propertyName in PropertyNames)
                                                                                                                         {
                                                                                                                             foreach (var methodPrefix in MethodPrefixes)
                                                                                                                             {
                                                                                                                                 An_issue_is_reported_for(@"
namespace BlaBla
{
    public interface ITestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private ITestMe " + methodPrefix + propertyName + @"() => null;
    }
}
");
                                                                                                                             }
                                                                                                                         }
                                                                                                                     }
                                                                                                                 });

        [Test]
        public void An_issue_is_reported_for_field_if_type_under_test_is_an_interface() => Assert.Multiple(() =>
                                                                                                                {
                                                                                                                    foreach (var testFixture in TestFixtures)
                                                                                                                    {
                                                                                                                        foreach (var fieldName in FieldNames)
                                                                                                                        {
                                                                                                                            An_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public interface ITestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private ITestMe " + fieldName + @";
    }
}
");
                                                                                                                        }
                                                                                                                    }
                                                                                                                });

        [Test]
        public void An_issue_is_reported_for_localVariable_if_type_under_test_is_an_interface() => Assert.Multiple(() =>
                                                                                                                        {
                                                                                                                            foreach (var testFixture in TestFixtures)
                                                                                                                            {
                                                                                                                                foreach (var test in Tests)
                                                                                                                                {
                                                                                                                                    foreach (var variableName in VariableNames)
                                                                                                                                    {
                                                                                                                                        An_issue_is_reported_for(@"
namespace BlaBla
{
    public interface ITestMe
    {
    }

    public class TestMe : ITestMe
    {
    }

    [" + testFixture + @"]
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
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        });

        protected override string GetDiagnosticId() => MiKo_3121_ObjectUnderTestIsNoInterfaceAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3121_ObjectUnderTestIsNoInterfaceAnalyzer();
    }
}