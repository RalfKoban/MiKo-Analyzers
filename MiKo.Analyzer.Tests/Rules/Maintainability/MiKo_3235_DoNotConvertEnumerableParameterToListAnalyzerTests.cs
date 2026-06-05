using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3235_DoNotConvertEnumerableParameterToListAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Calls = ["ToList", "ToArray"];

        [Test]
        public void No_issue_is_reported_for_methods_with_no_IEnumerable_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_IEnumerable_parameter() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_methods_with_IEnumerable_parameter_and_unrelated_call_([ValueSource(nameof(Calls))] string call) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public List<int> DoSomething(IEnumerable<int> values)
        {
            List<int> results = new List<int>();

            foreach (var value in values)
            {
                if (value > 42)
                {
                    results.Add(value);
                }
            }

            return results." + call + @"();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_methods_with_IEnumerable_parameter_and_call_([ValueSource(nameof(Calls))] string call) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            var valuesList = values." + call + @"();

            foreach (var value in valuesList)
            {
                Console.WriteLine(value);
            }
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3235_DoNotConvertEnumerableParameterToListAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3235_DoNotConvertEnumerableParameterToListAnalyzer();
    }
}