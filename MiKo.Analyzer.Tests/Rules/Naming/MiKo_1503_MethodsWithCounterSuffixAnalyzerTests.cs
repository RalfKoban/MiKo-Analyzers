﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1503_MethodsWithCounterSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_value_type_method_without_Counter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private int Something() => 42;
  }
}
");

        [Test]
        public void No_issue_is_reported_for_reference_type_method_with_Counter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private object SomeCounter() => 42;
  }
}
");

        [Test]
        public void No_issue_is_reported_for_reference_type_local_function_with_Counter_in_its_name() => No_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private void DoSomething()
      {
        object SomeCounter() => 42;
      }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_Counter_suffix() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
  [TestFixture]
  public class TestMe
  {
      private int SomeCounter() => 42;
  }
}
");

        [Test]
        public void An_issue_is_reported_for_value_type_local_function_with_Counter_in_its_name() => An_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private void DoSomething()
      {
          int SomeCounter() => 42;
      }
  }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Counter_suffix() => An_issue_is_reported_for(@"
namespace Bla
{
  public class TestMe
  {
      private int SomeCounter() => 42;
  }
}
");

        [TestCase("SomeCounter", "SomeCount")]
        [TestCase("IncrementCounter", "Increment")]
        [TestCase("DecrementCounter", "Decrement")]
        public void Code_gets_fixed_for_method_(string originalName, string fixedName)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        private int ###() => 42;
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        [TestCase("SomeCounter", "SomeCount")]
        [TestCase("IncrementCounter", "Increment")]
        [TestCase("DecrementCounter", "Decrement")]
        public void Code_gets_fixed_for_local_function_(string originalName, string fixedName)
        {
            const string Template = @"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething()
        {
            int ###() => 42;
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1503_MethodsWithCounterSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1503_MethodsWithCounterSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1503_CodeFixProvider();
    }
}