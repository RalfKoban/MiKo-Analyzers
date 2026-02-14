using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2002_EventArgsSummaryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_EventArgs_class_without_documentation() => No_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_EventArgs_name_suffix_not_inheriting_from_EventArgs() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Does something.
/// </summary>
public class MyEventArgs // simply has event args indicator but does not inherit from EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_unsealed_EventArgs_class_with_summary_providing_data_for_specific_event() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Provides data for the <see cref=""My"" /> event.
/// </summary>
public class MyEventArgs : EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_sealed_EventArgs_class_with_summary_providing_data_for_specific_event_and_inheritance_restriction() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Provides data for the <see cref=""My"" /> event.
/// This class cannot be inherited.
/// </summary>
public sealed class MyEventArgs : EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_EventArgs_class_with_summary_providing_data_for_multiple_events() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Provides data for some events.
/// </summary>
public class MyEventArgs : EventArgs
{
}
");

        [TestCase("Does something.")]
        [TestCase("Provides some stuff for the event.")]
        [TestCase("Provides data for the event.")]
        public void An_issue_is_reported_for_EventArgs_class_with_summary_not_following_standard_phrase_(string comment) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// " + comment + @"
/// </summary>
public class MyEventArgs : EventArgs
{
}
");

        [Test]
        public void Code_gets_fixed_to_replace_summary_with_standard_provides_data_phrase_and_TODO_event_reference()
        {
            const string OriginalCode = @"
using System;

/// <summary>
/// Some comment.
/// </summary>
public class MyEventArgs : EventArgs
{
}
";

            const string FixedCode = @"
using System;

/// <summary>
/// Provides data for the <see cref=""TODO""/> event.
/// </summary>
public class MyEventArgs : EventArgs
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_replace_summary_while_preserving_remarks_section()
        {
            const string OriginalCode = @"
using System;

/// <summary>
/// Some comment.
/// </summary>
/// <remarks>
/// These are some remarks.
/// </remarks>
public class MyEventArgs : EventArgs
{
}
";

            const string FixedCode = @"
using System;

/// <summary>
/// Provides data for the <see cref=""TODO""/> event.
/// </summary>
/// <remarks>
/// These are some remarks.
/// </remarks>
public class MyEventArgs : EventArgs
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_to_replace_summary_phrase_while_extracting_and_preserving_event_reference()
        {
            const string OriginalCode = @"
using System;

/// <summary>
/// Event argument which is used in the <see cref=""IMyInterface.MyEvent""/> event.
/// </summary>
/// <remarks>
/// These are some remarks.
/// </remarks>
public class MyEventArgs : EventArgs
{
}

public interface IMyInterface
{
    event EventHandler<MyEventArgs> MyEvent;
}
";

            const string FixedCode = @"
using System;

/// <summary>
/// Provides data for the <see cref=""IMyInterface.MyEvent""/> event.
/// </summary>
/// <remarks>
/// These are some remarks.
/// </remarks>
public class MyEventArgs : EventArgs
{
}

public interface IMyInterface
{
    event EventHandler<MyEventArgs> MyEvent;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2002_EventArgsSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2002_EventArgsSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2002_CodeFixProvider();
    }
}