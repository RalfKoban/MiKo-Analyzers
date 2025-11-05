using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6068_PropertyPatternInConditionIsOnSameLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_sub_pattern_in_if_condition_that_is_on_single_line() => No_issue_is_reported_for(@"
using System;
using System.Xml;

public class TestMe
{
    public void DoSomething(XmlReader reader)
    {
        while (xmlReader.Read())
        {
            if (xmlReader is { NodeType: XmlNodeType.Element, Name: ""Log"" })
            {
                return;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_sub_pattern_in_if_condition_that_spans_multiple_line() => An_issue_is_reported_for(@"
using System;
using System.Xml;

public class TestMe
{
    public void DoSomething(XmlReader reader)
    {
        while (xmlReader.Read())
        {
            if (xmlReader is {
                                  NodeType: XmlNodeType.Element,
                                  Name: ""Log""
                             })
            {
                return;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_sub_pattern_in_if_condition_that_spans_multiple_line()
        {
            const string OriginalCode = """
                                        using System;
                                        using System.Xml;

                                        public class TestMe
                                        {
                                            public void DoSomething(XmlReader reader)
                                            {
                                                while (xmlReader.Read())
                                                {
                                                    if (xmlReader is {
                                                                          NodeType: XmlNodeType.Element,
                                                                          Name: "Log"
                                                                     })
                                                    {
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        """;

            const string FixedCode = """
                                        using System;
                                        using System.Xml;

                                        public class TestMe
                                        {
                                            public void DoSomething(XmlReader reader)
                                            {
                                                while (xmlReader.Read())
                                                {
                                                    if (xmlReader is { NodeType: XmlNodeType.Element, Name: "Log" })
                                                    {
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_recursive_pattern_in_if_condition_that_spans_multiple_line()
        {
            const string OriginalCode = """
                                        using System;
                                        using System.Xml;

                                        public class TestMe
                                        {
                                            public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda)
                                            {
                                                if (propertyLambda.Body is not MemberExpression
                                                    {
                                                        Member: PropertyInfo
                                                        {
                                                            ReflectedType: { } reflectedType,
                                                            Name: { } name
                                                        }
                                                    })
                                                {
                                                    throw new ArgumentException($"Expression '{propertyLambda}' does not refer to a property.");
                                                }
                                            }
                                        }
                                        """;

            const string FixedCode = """
                                        using System;
                                        using System.Xml;

                                        public class TestMe
                                        {
                                            public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyLambda)
                                            {
                                                if (propertyLambda.Body is not MemberExpression { Member: PropertyInfo { ReflectedType: { } reflectedType, Name: { } name } })
                                                {
                                                    throw new ArgumentException($"Expression '{propertyLambda}' does not refer to a property.");
                                                }
                                            }
                                        }
                                        """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6068_PropertyPatternInConditionIsOnSameLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6068_PropertyPatternInConditionIsOnSameLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6068_CodeFixProvider();
    }
}