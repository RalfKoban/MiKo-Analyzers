using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1017_GetSetPrefixedMethodsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidPrefixes = { string.Empty, "Get", "Set", "GetCanceled", "SetCanceled", "HasCanceled", "GetHashCode", "SetHash", "HasExited" };

        private static readonly string[] InvalidPrefixes = { "GetIs", "SetIs", "GetCan", "SetCan", "GetHas", "SetHas", "CanHas", "CanIs", "HasIs", "HasCan", "IsCan", "IsHas", "GetExists", "CanExists", "HasExists", "IsExists" };

        [Test]
        public void No_issue_is_reported_for_method_with_prefix_([ValueSource(nameof(ValidPrefixes))] string prefix) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + prefix + @"Something()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_with_prefix_([ValueSource(nameof(ValidPrefixes))] string prefix) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void " + prefix + @"Something() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_special_CanOpen_method_([Values("GetCanOpenManager", "IsCanOpenManager", "SetCanOpenManager", "HasCanOpenManager")] string methodName) => No_issue_is_reported_for(@"
namespace My.CanOpen.Namespace
{
    public class TestMe
    {
        public void " + methodName + @"()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameterless_method_with_prefix_([ValueSource(nameof(InvalidPrefixes))] string prefix) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + prefix + @"Something()
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_parameter_method_with_prefix_([ValueSource(nameof(InvalidPrefixes))] string prefix) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + prefix + @"Something(object o)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_local_function_with_prefix_([ValueSource(nameof(InvalidPrefixes))] string prefix) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        void " + prefix + @"Something(object o) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_DependencyObject_parameters() => No_issue_is_reported_for(@"
using System.Windows;

public class TestMe
{
    public void GetIsEnabled(DependencyObject do)
    {
    }

    public void SetIsEnabled(DependencyObject do)
    {
    }
}
");

        // "GetCan",
        // "GetHas",
        // "GetIs",
        // "GetExists",
        // "SetCan",
        // "SetHas",
        // "SetIs",
        // "SetExists",
        // "CanHas",
        // "CanIs",
        // "CanExists",
        // "HasCan",
        // "HasIs",
        // "HasExists",
        // "IsCan",
        // "IsHas",
        // "IsExists",
        [TestCase("GetCanOpen", "CanOpen")]
        [TestCase("GetHasItem", "HasItem")]
        [TestCase("GetIsDirectory", "IsDirectory")]
        [TestCase("GetExists", "Exists")]
        [TestCase("HasCanOpen", "CanOpen")]
        [TestCase("HasIsDirectory", "IsDirectory")]
        [TestCase("HasExists", "Exists")]
        [TestCase("IsCanOpen", "CanOpen")]
        [TestCase("IsHasItem", "HasItem")]
        [TestCase("IsExists", "Exists")]
        public void Code_gets_fixed_(string originalMethodName, string fixedMethodName)
        {
            const string Template = @"
public class TestMe
{
    public void ###()
    {
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalMethodName), Template.Replace("###", fixedMethodName));
        }

        protected override string GetDiagnosticId() => MiKo_1017_GetSetPrefixedMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1017_GetSetPrefixedMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1017_CodeFixProvider();
    }
}