using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3055_ViewModelImplementsINotifyPropertyChangedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_interface() => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public interface IViewModel
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_type() => No_issue_is_reported_for(@"
using System.Windows;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class SomeTestsForViewModel
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_name_([Values("ViewModelFactory", "TestMe")] string name) => No_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class " + name + @"
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_view_model_class_that_implements_INotifyPropertyChanged() => No_issue_is_reported_for(@"
using System.Windows;
using System.ComponentModel;

namespace Bla
{
    public class TestMeViewModel : INotifyPropertyChanged
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_view_model_class_that_implements_INotifyPropertyChanged_via_base_class() => No_issue_is_reported_for(@"
using System.Windows;
using System.ComponentModel;

namespace Bla
{
    public class SomeBaseClass : INotifyPropertyChanged
    {
    }

    public class TestMeViewModel : SomeBaseClass
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_view_model_class_that_does_not_implement_INotifyPropertyChanged() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class TestMeViewModel
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_view_model_class_that_does_not_implement_INotifyPropertyChanged_and_also_not_its_base_classes() => An_issue_is_reported_for(@"
using System.Windows;

namespace Bla
{
    public class SomeBaseClass
    {
    }

    public class TestMeViewModel : SomeBaseClass
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3055_ViewModelImplementsINotifyPropertyChangedAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3055_ViewModelImplementsINotifyPropertyChangedAnalyzer();
    }
}