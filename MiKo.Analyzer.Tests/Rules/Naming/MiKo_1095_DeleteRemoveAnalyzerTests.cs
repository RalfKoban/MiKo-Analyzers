using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1095_DeleteRemoveAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_Delete_Remove_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}");

        [TestCase("DeleteRemovedUser")]
        [TestCase("RemoveDeletedUser")]
        [TestCase("RemoveAndDeleteUser")]
        [TestCase("DeleteAndRemoveUser")]
        public void No_issue_is_reported_for_both_Delete_and_Remove_in_name_of_type_(string name) => No_issue_is_reported_for(@"
using System;

public class " + name + @"TestMe
{
    public void DoSomething()
    {
    }
}");

        [TestCase("Delete", "Removes")]
        [TestCase("Remove", "Deletes")]
        public void An_issue_is_reported_for_both_Delete_and_Remove_in_name_of_type_and_documentation_(string name, string comment) => An_issue_is_reported_for(@"
using System;

/// <summary>
/// " + comment + @" a user.
/// </summary>
public class " + name + @"TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_Delete_Remove_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}");

        [TestCase("DeleteRemovedUser")]
        [TestCase("RemoveDeletedUser")]
        [TestCase("RemoveAndDeleteUser")]
        [TestCase("DeleteAndRemoveUser")]
        public void No_issue_is_reported_for_both_Delete_and_Remove_in_name_of_method_(string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void " + name + @"()
    {
    }
}");

        [TestCase("Delete", "Removes")]
        [TestCase("Remove", "Deletes")]
        public void An_issue_is_reported_for_both_Delete_and_Remove_in_name_of_method_and_documentation_(string name, string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// " + comment + @" a user.
    /// </summary>
    public void " + name + @"User()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_Delete_Remove_event() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public EventHandler SomeEvent;
}");

        [TestCase("DeleteRemovedUser")]
        [TestCase("RemoveDeletedUser")]
        [TestCase("RemoveAndDeleteUser")]
        [TestCase("DeleteAndRemoveUser")]
        public void No_issue_is_reported_for_both_Delete_and_Remove_in_name_of_event_(string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public EventHandler " + name + @"Event;
}");

        [TestCase("Delete", "Removes")]
        [TestCase("Remove", "Deletes")]
        public void An_issue_is_reported_for_both_Delete_and_Remove_in_name_of_event_and_documentation_(string name, string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Occurs when " + comment + @" a user.
    /// </summary>
    public EventHandler " + name + @"Event;
}
");

        [Test]
        public void No_issue_is_reported_for_non_Delete_Remove_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool IsSomething { get; set; }
}");

        [TestCase("DeleteRemovedUser")]
        [TestCase("RemoveDeletedUser")]
        [TestCase("RemoveAndDeleteUser")]
        [TestCase("DeleteAndRemoveUser")]
        public void No_issue_is_reported_for_both_Delete_and_Remove_in_name_of_property_(string name) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool Is" + name + @"Something { get; set; }
}");

        [TestCase("Delete", "Removes")]
        [TestCase("Remove", "Deletes")]
        public void An_issue_is_reported_for_both_Delete_and_Remove_in_name_of_property_and_documentation_(string name, string comment) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    /// <summary>
    /// Gets or sets a value indicating whether some " + comment + @" something.
    /// </summary>
    public bool Is" + name + @"Something { get; set; }
}
");

        protected override string GetDiagnosticId() => MiKo_1095_DeleteRemoveAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1095_DeleteRemoveAnalyzer();
    }
}