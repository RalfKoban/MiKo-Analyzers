using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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

        [Test]
        public void Code_gets_fixed_for_Cinch_CreateArgs()
        {
            const string OriginalCode = @"
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
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class TestMe
    {
        public string Something { get; set; }

        private PropertyChangedEventArgs e = new PropertyChangedEventArgs(nameof(Something));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Cinch_GetPropertyName()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class TestMe
    {
        public string Something { get; set; }

        public void DoSomething()
        {
            var x = ObservableHelper.GetPropertyName<TestMe>(_ => _.Something);
            DoSomething(x);
        }

        public void DoSomething(string s)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class TestMe
    {
        public string Something { get; set; }

        public void DoSomething()
        {
            var x = nameof(Something);
            DoSomething(x);
        }

        public void DoSomething(string s)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Cinch_GetPropertyName_if_name_comes_from_different_type()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class NameFromMe
    {
        public int SomeData { get; set; }
    }

    public class TestMe
    {
        public string Something { get; set; }

        public void DoSomething()
        {
            var x = ObservableHelper.GetPropertyName<NameFromMe>(_ => _.SomeData);
            DoSomething(x);
        }

        public void DoSomething(string s)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class NameFromMe
    {
        public int SomeData { get; set; }
    }

    public class TestMe
    {
        public string Something { get; set; }

        public void DoSomething()
        {
            var x = nameof(NameFromMe.SomeData);
            DoSomething(x);
        }

        public void DoSomething(string s)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Cinch_GetPropertyName_if_name_comes_from_base_class()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class NameFromMe
    {
        public int SomeData { get; set; }
    }

    public class TestMe : NameFromMe
    {
        public string Something { get; set; }

        public void DoSomething()
        {
            var x = ObservableHelper.GetPropertyName<NameFromMe>(_ => _.SomeData);
            DoSomething(x);
        }

        public void DoSomething(string s)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public class NameFromMe
    {
        public int SomeData { get; set; }
    }

    public class TestMe : NameFromMe
    {
        public string Something { get; set; }

        public void DoSomething()
        {
            var x = nameof(SomeData);
            DoSomething(x);
        }

        public void DoSomething(string s)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Cinch_GetPropertyName_if_name_comes_from_implemented_interface()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public interface INameFromMe
    {
        int SomeData { get; set; }
    }

    public class TestMe : INameFromMe
    {
        public string Something { get; set; }

        public int SomeData { get; set; }

        public void DoSomething()
        {
            var x = ObservableHelper.GetPropertyName<INameFromMe>(_ => _.SomeData);
            DoSomething(x);
        }

        public void DoSomething(string s)
        {
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;

using Cinch;

namespace Bla
{
    public interface INameFromMe
    {
        int SomeData { get; set; }
    }

    public class TestMe : INameFromMe
    {
        public string Something { get; set; }

        public int SomeData { get; set; }

        public void DoSomething()
        {
            var x = nameof(SomeData);
            DoSomething(x);
        }

        public void DoSomething(string s)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3032_CodeFixProvider();
    }
}