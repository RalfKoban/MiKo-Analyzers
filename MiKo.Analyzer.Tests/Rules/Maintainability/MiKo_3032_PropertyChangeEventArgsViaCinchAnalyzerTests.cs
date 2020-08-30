using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correct_usage_inside_property() => No_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public int Something
        {
            get
            {
                return m_something;
            }

            set
            {
                var x = new PropertyChangedEventArgs(nameof(Something));
            }
        }

        private int m_something;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_usage_on_field_assignment() => No_issue_is_reported_for(@"

using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe
    {
        public int Something { get; set; }

        private PropertyChangedEventArgs e = new PropertyChangedEventArgs(nameof(Something));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Cinch_CreateArgs_usage_in_setter() => An_issue_is_reported_for(@"

using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class TestMe
    {
        public int Something
        {
            get
            {
                return m_something;
            }

            set
            {
                var x = ObservableHelper.CreateArgs<TestMe>(_ => _.Something);
            }
        }

        private int m_something;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Cinch_CreateArgs_usage_at_field_assignment() => An_issue_is_reported_for(@"

using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class TestMe
    {
        public string Something { get; set; }

        private PropertyChangedEventArgs e = ObservableHelper.CreateArgs<TestMe>(_ => _.Something);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Cinch_GetPropertyName_usage_in_setter() => An_issue_is_reported_for(@"

using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class TestMe
    {
        public int Something
        {
            get
            {
                return m_something;
            }

            set
            {
                var x = ObservableHelper.GetPropertyName<TestMe>(_ => _.Something);
            }
        }

        private int m_something;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer();
    }
}