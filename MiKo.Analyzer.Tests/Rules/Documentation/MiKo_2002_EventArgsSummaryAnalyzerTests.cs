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
        public void No_issue_is_reported_for_uncommented_EventArgs() => No_issue_is_reported_for(@"
using System;

public class MyEventArgs : EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_EventArgs() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Does something.
/// </summary>
public class MyEventArgs // simply has event args indicator but does not inherit from EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_unsealed_EventArgs() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Provides data for the <see cref=""My"" /> event.
/// </summary>
public class MyEventArgs : EventArgs
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_sealed_EventArgs() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_correctly_commented_multi_EventArgs() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_incorrectly_commented_EventArgs_(string comment) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// " + comment + @"
/// </summary>
public class MyEventArgs : EventArgs
{
}
");

        [Test]
        public void Code_gets_fixed()
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
        public void Code_gets_fixed_but_keeps_Remarks()
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
        public void Code_gets_fixed_for_comment_with_link_to_event()
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