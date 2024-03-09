using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3218_DoNotDefineExtensionMethodsInUnexpectedPlacesAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Visibilities = { "private", "protected", "internal", "public" };

        [Test]
        public void No_issue_is_reported_for_empty_type() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_valid_extension_method_with_visibility_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
public class TestMeExtensions
{
    " + visibility + @" static int ExtendMe(this int value) => value;
}
");

        [Test]
        public void No_issue_is_reported_for_valid_static_method_with_visibility_([ValueSource(nameof(Visibilities))] string visibility) => No_issue_is_reported_for(@"
public class TestMe
{
    " + visibility + @" static int ExtendMe(int value) => value;
}
");

        [Test]
        public void An_issue_is_reported_for_extension_method_with_visibility_([ValueSource(nameof(Visibilities))] string visibility) => An_issue_is_reported_for(@"
public class TestMe
{
    " + visibility + @" static int ExtendMe(this int value) => value;
}
");

        protected override string GetDiagnosticId() => MiKo_3218_DoNotDefineExtensionMethodsInUnexpectedPlacesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3218_DoNotDefineExtensionMethodsInUnexpectedPlacesAnalyzer();
    }
}