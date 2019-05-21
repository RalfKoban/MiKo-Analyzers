using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1050_ReturnValueLocalVariableAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Fitting = { "myVariable" };
        private static readonly string[] NonFitting = CreateNonFitting();

        [Test]
        public void No_issue_is_reported_for_variable_with_fitting_name([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        var " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_with_non_fitting_name([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        var " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_with_fitting_name([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return 42;
            default: return -1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_with_non_fitting_name([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return 42;
            default: return -1;
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1050_ReturnValueLocalVariableAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1050_ReturnValueLocalVariableAnalyzer();

        [ExcludeFromCodeCoverage]
        private static string[] CreateNonFitting()
        {
            var terms = new[] { "ret", "retVal", "retVals", "returnVal", "returnVals", "returnValue", "returnValues", "ret1", "ret2", "retVal3", "returnValue4", "retVal_5", "resultList" };

            var nonFitting = new HashSet<string>(terms);
            foreach (var _ in terms)
            {
                nonFitting.Add(_.ToLowerInvariant());
                nonFitting.Add(_.ToUpperInvariant());
            }

            return nonFitting.OrderBy(_ => _).ToArray();
        }
    }
}