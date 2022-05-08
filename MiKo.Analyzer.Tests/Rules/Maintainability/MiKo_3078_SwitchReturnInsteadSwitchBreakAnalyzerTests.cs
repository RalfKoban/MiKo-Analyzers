using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    internal sealed class MiKo_3078_SwitchReturnInsteadSwitchBreakAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_factory_that_directly_returns_value_instead_of_assigning_it_to_variable() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Create(StringComparison value)
    {
        switch (value)
        {
            case StringComparison.Ordinal:
                return new object();
                
            case StringComparison.OrdinalIgnoreCase:
                return new object();

            default:
                throw new NotSupportedException();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_factory_that_assigns_return_value_to_variable_instead_of_returning_it_directly() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Create(StringComparison value)
    {
        object result;

        switch (value)
        {
            case StringComparison.Ordinal:
                result = new object();
                break;

            case StringComparison.OrdinalIgnoreCase:
                result = new object();
                break;

            default:
                throw new NotSupportedException();
        }

        return result;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_factory_that_assigns_return_value_to_variable_inside_block_nstead_of_returning_it_directly() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Create(StringComparison value)
    {
        object result;

        switch (value)
        {
            case StringComparison.Ordinal:
            {
                result = new object();
                break;
            }

            case StringComparison.OrdinalIgnoreCase:
            {
                result = new object();
                break;
            }

            default:
                throw new NotSupportedException();
        }

        return result;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_factory_that_assigns_return_value_to_variable_inside_condition_block_instead_of_returning_it_directly() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Create(StringComparison value, bool useNull)
    {
        object result;

        if (useNull)
        {
            return null;
        }
        else
        {
            switch (value)
            {
                case StringComparison.Ordinal:
                    result = new object();
                    break;

                case StringComparison.OrdinalIgnoreCase:
                    result = new object();
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        return result;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_that_determines_multiple_conditions_and_keeps_information_in_different_variables() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool HasIssue(StringComparison[] values)
    {
        bool ordinal = false;
        bool ordinalIgnoreCase = false;

        foreach (var value in values)
        {
            switch (value)
            {
                case StringComparison.Ordinal:
                    ordinal = true;
                    break;

                case StringComparison.OrdinalIgnoreCase:
                    ordinalIgnoreCase = true;
                    break;
            }

            return ordinal && ordinalIgnoreCase is false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_that_determines_multiple_conditions_and_calls_other_methods_only() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(StringComparison[] values)
    {
        foreach (var value in values)
        {
            switch (value)
            {
                case StringComparison.Ordinal:
                    DoOrdinal();
                    break;

                case StringComparison.OrdinalIgnoreCase:
                    DoOrdinalIgnoreCase();
                    break;
            }
        }
    }

    private void DoOrdinal()
    { }

    private void DoOrdinalIgnoreCase()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_determines_single_condition_and_keeps_information_in_same_variable() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool HasIssue(StringComparison[] values)
    {
        bool ordinal = false;

        foreach (var value in values)
        {
            switch (value)
            {
                case StringComparison.Ordinal:
                    ordinal = true;
                    break;

                case StringComparison.OrdinalIgnoreCase:
                    ordinal = true;
                    break;
            }

            return ordinal;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_that_handles_NotifyCollectionChangedAction_action() => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Specialized;

public class TestMe
{
    public void Handle(NotifyCollectionChangedEventArgs e)
    {
        var action = e.Action;
        switch (action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Reset:
            {
                var sum = 0;

                foreach (var i in e.NewItems.OfType<int>())
                {
                    sum += i;
                }

                break;
            }
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3078_SwitchReturnInsteadSwitchBreakAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3078_SwitchReturnInsteadSwitchBreakAnalyzer();
    }
}
