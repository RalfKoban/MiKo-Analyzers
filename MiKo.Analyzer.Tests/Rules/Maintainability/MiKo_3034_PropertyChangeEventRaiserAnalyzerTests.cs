using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3034_PropertyChangeEventRaiserAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_usage_in_method() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_no_usage_in_property_([Values("nameof(Something)", "\"Something\"")] string propertyName) => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Something
        {
            get { return m_something; }
            set
            {
                m_something = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(" + propertyName + @");
            }
        }

        private int m_something;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_field_assignment() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrect_usage_in_method() => An_issue_is_reported_for(@"
using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_normal_method_and_missing_using_is_placed_after_all_usings()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_normal_method_and_missing_using_is_placed_before_using_with_higher_starting_letter()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;
using Xyz;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xyz;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_normal_method_and_existing_using_is_kept()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_default_parameter()
        {
            const string OriginalCode = @"
using System;
using System.ComponentModel;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bla
{
    public class TestMe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3034_PropertyChangeEventRaiserAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3034_PropertyChangeEventRaiserAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3034_CodeFixProvider();
    }
}