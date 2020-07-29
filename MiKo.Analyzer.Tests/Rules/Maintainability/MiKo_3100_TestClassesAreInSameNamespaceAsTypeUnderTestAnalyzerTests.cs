using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] PropertyNames =
            {
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
            };

        private static readonly string[] FieldNames =
            {
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
            };

        private static readonly string[] VariableNames =
            {
                "objectUnderTest",
                "subjectUnderTest",
                "unitUnderTest",
                "testCandidate",
                "testObject",
                "sut",
                "uut",
            };

        private static readonly string[] MethodPrefixes =
            {
                "Get",
                "Create",
            };

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
        public void No_issue_is_reported_for_empty_test_class_([ValueSource(nameof(TestFixtures))] string testFixture) => No_issue_is_reported_for(@"
namespace BlaBla
{
    [" + testFixture + @"]
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_property_if_test_class_and_class_under_test_are_in_same_namespace() => Assert.Multiple(() =>
                                                                                                                                        {
                                                                                                                                            foreach (var testFixture in TestFixtures)
                                                                                                                                            {
                                                                                                                                                foreach (var propertyName in PropertyNames)
                                                                                                                                                {
                                                                                                                                                    No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
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
        public void No_issue_is_reported_for_method_if_test_class_and_class_under_test_are_in_same_namespace_([ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => Assert.Multiple(() =>
                                   {
                                       foreach (var testFixture in TestFixtures)
                                       {
                                           foreach (var propertyName in PropertyNames)
                                           {
                                               No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
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
                                   });

        [Test]
        public void No_issue_is_reported_for_field_if_test_class_and_class_under_test_are_in_same_namespace() => Assert.Multiple(() =>
                                                                                                                                     {
                                                                                                                                         foreach (var testFixture in TestFixtures)
                                                                                                                                         {
                                                                                                                                             foreach (var fieldName in FieldNames)
                                                                                                                                             {
                                                                                                                                                 No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
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
        public void No_issue_is_reported_for_localVariable_if_test_class_and_class_under_test_are_in_same_namespace() => Assert.Multiple(() =>
                                                                                                                                             {
                                                                                                                                                 foreach (var testFixture in TestFixtures)
                                                                                                                                                 {
                                                                                                                                                     foreach (var test in Tests)
                                                                                                                                                     {
                                                                                                                                                         foreach (var variableName in VariableNames)
                                                                                                                                                         {
                                                                                                                                                             No_issue_is_reported_for(@"
namespace BlaBla.BlaBlubb
{
    public class TestMe
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
        public void An_issue_is_reported_for_method_if_test_class_and_class_under_test_are_in_different_namespaces_([Values("Get", "Create")] string methodPrefix)
            => Assert.Multiple(() =>
                                    {
                                        foreach (var testFixture in TestFixtures)
                                        {
                                            foreach (var propertyName in PropertyNames)
                                            {
                                                An_issue_is_reported_for(@"
namespace BlaBla
{
    public class TestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    using BlaBla;

    [" + testFixture + @"]
    public class TestMeTests
    {
        private TestMe " + methodPrefix + propertyName + @"() => null;
    }
}
");
                                            }
                                        }
                                    });

        [Test]
        public void An_issue_is_reported_for_property_if_test_class_and_class_under_test_are_in_different_namespaces() => Assert.Multiple(() =>
                                                                                                                                              {
                                                                                                                                                  foreach (var testFixture in TestFixtures)
                                                                                                                                                  {
                                                                                                                                                      foreach (var propertyName in PropertyNames)
                                                                                                                                                      {
                                                                                                                                                          An_issue_is_reported_for(@"
namespace BlaBla
{
    public class TestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    using BlaBla;

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
        public void An_issue_is_reported_for_field_if_test_class_and_class_under_test_are_in_different_namespaces() => Assert.Multiple(() =>
                                                                                                                                           {
                                                                                                                                               foreach (var testFixture in TestFixtures)
                                                                                                                                               {
                                                                                                                                                   foreach (var fieldName in FieldNames)
                                                                                                                                                   {
                                                                                                                                                       An_issue_is_reported_for(@"
namespace BlaBla
{
    public class TestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    using BlaBla;

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
        public void An_issue_is_reported_for_local_variable_if_test_class_and_class_under_test_are_in_different_namespaces() => Assert.Multiple(() =>
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
    public class TestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    using BlaBla;

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
        public void No_issue_is_reported_for_method_if_test_class_and_returned_class_under_test_are_in_same_namespace_([ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => Assert.Multiple(() =>
                                   {
                                       foreach (var testFixture in TestFixtures)
                                       {
                                           foreach (var propertyName in PropertyNames)
                                           {
                                               No_issue_is_reported_for(@"
namespace BlaBla
{
    public class BaseTestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    public class TestMe : BaseTestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private BaseTestMe " + methodPrefix + propertyName + @"() => new TestMe();
    }
}
");
                                           }
                                       }
                                   });

        [Test]
        public void An_issue_is_reported_for_method_if_test_class_and_returned_class_under_test_are_in_different_namespace_([ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => Assert.Multiple(() =>
                                   {
                                       foreach (var testFixture in TestFixtures)
                                       {
                                           foreach (var propertyName in PropertyNames)
                                           {
                                               An_issue_is_reported_for(@"
namespace BlaBla
{
    public class BaseTestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    public class TestMe : BaseTestMe
    {
    }

}

namespace BlaBla.BlaBlubb.Tests
{
    [" + testFixture + @"]
    public class TestMeTests
    {
        private BaseTestMe " + methodPrefix + propertyName + @"() => new TestMe();
    }
}
");
                                           }
                                       }
                                   });

        [Test]
        public void No_issue_is_reported_for_method_if_variable_that_is_test_class_and_returned_class_under_test_are_in_same_namespace_([ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => Assert.Multiple(() =>
                                   {
                                       foreach (var testFixture in TestFixtures)
                                       {
                                           foreach (var propertyName in PropertyNames)
                                           {
                                               No_issue_is_reported_for(@"
namespace BlaBla
{
    public class BaseTestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    public class TestMe : BaseTestMe
    {
    }

    [" + testFixture + @"]
    public class TestMeTests
    {
        private BaseTestMe " + methodPrefix + propertyName + @"()
        {
            BaseTestMe me = new TestMe();
            return me;
        }
    }
}
");
                                           }
                                       }
                                   });

        [Test]
        public void An_issue_is_reported_for_method_if_variable_that_is_test_class_and_returned_class_under_test_are_in_different_namespace_([ValueSource(nameof(MethodPrefixes))] string methodPrefix)
            => Assert.Multiple(() =>
                                   {
                                       foreach (var testFixture in TestFixtures)
                                       {
                                           foreach (var propertyName in PropertyNames)
                                           {
                                               An_issue_is_reported_for(@"
namespace BlaBla
{
    public class BaseTestMe
    {
    }
}

namespace BlaBla.BlaBlubb
{
    public class TestMe : BaseTestMe
    {
    }

}

namespace BlaBla.BlaBlubb.Tests
{
    [" + testFixture + @"]
    public class TestMeTests
    {
        private BaseTestMe " + methodPrefix + propertyName + @"()
        {
            BaseTestMe me = new TestMe();
            return me;
        }
    }
}
");
                                           }
                                       }
                                   });

        protected override string GetDiagnosticId() => MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3100_TestClassesAreInSameNamespaceAsTypeUnderTestAnalyzer();
    }
}
