using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_event_handling_method() => No_issue_is_reported_for(@"

using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_method() => No_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void OnWhateverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_not_starting_with_On() => An_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void WhateverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_not_ending_with_Changed() => An_issue_is_reported_for(@"
namespace System.Windows
{
    public struct DependencyPropertyChangedEventArgs
    {
    }
}

namespace Bla
{
    using System;
    using System.Windows;

    public class TestMe
    {
        public void OnWhatever(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
    }
}
");

        [TestCase("OnWhatever", "OnWhateverChanged")]
        [TestCase("OnWhateverChange", "OnWhateverChanged")]
        [TestCase("Whatever", "OnWhateverChanged")]
        [TestCase("WhateverChange", "OnWhateverChanged")]
        [TestCase("WhateverChanged", "OnWhateverChanged")]
        [TestCase("WhateverProperty", "OnWhateverChanged")]
        [TestCase("WhateverPropertyChange", "OnWhateverChanged")]
        public void Code_gets_fixed_for_method_(string originalName, string fixedName)
        {
            const string Template = """

                                    namespace System.Windows
                                    {
                                        public struct DependencyPropertyChangedEventArgs
                                        {
                                        }
                                    }

                                    namespace Bla
                                    {
                                        using System;
                                        using System.Windows;

                                        public class TestMe
                                        {
                                            public void ###(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
                                        }
                                    }

                                    """;

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1530_DependencyPropertyChangeHandlingMethodNamesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1530_CodeFixProvider();
    }
}