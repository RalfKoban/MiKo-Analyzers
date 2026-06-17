using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    internal sealed class MiKo_3080_SwitchReturnInsteadSwitchBreakAnalyzerTests : CodeFixVerifier
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

        [Test]
        public void No_issue_is_reported_for_switch_that_determines_single_condition_and_keeps_information_in_same_variable_but_runs_in_while_loop_for_performance_optimizations() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool HasIssue(StringComparison[] values)
    {
        bool ordinal = false;

        while (true)
        {
            var value = values[0];

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

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void No_issue_is_reported_for_switch_in_top_level_statement_that_assigns_to_variable() => No_issue_is_reported_for(@"
using System;

object result;
var value = StringComparison.Ordinal;

switch (value)
{
    case StringComparison.Ordinal:
        result = new object();
        break;

    case StringComparison.OrdinalIgnoreCase:
        result = new object();
        break;
}
", languageVersion: LanguageVersion.CSharp9);

        [Test]
        public void No_issue_is_reported_for_switch_that_assigns_to_variable_in_only_one_of_multiple_sections() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Create(StringComparison value)
    {
        object result = null;

        switch (value)
        {
            case StringComparison.Ordinal:
                result = new object();
                break;

            case StringComparison.OrdinalIgnoreCase:
                break;

            default:
                break;
        }

        return result;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_switch_that_assigns_in_none_of_multiple_sections() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public object Create(StringComparison value)
    {
        object result = null;

        switch (value)
        {
            case StringComparison.Ordinal:
                break;

            case StringComparison.OrdinalIgnoreCase:
                break;

            default:
                break;
        }

        return result;
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
        public void An_issue_is_reported_for_factory_that_assigns_return_value_to_variable_inside_block_instead_of_returning_it_directly() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_switch_that_determines_single_condition_and_keeps_information_in_same_out_parameter() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoStuff(StringComparison value, out bool ordinal)
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
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_determines_single_condition_and_keeps_information_in_same_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool Ordinal { get; set; }

    public void DoStuff(StringComparison value)
    {
        switch (value)
        {
            case StringComparison.Ordinal:
                Ordinal = true;
                break;

            case StringComparison.OrdinalIgnoreCase:
                Ordinal = true;
                break;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_determines_single_condition_and_keeps_information_in_same_property_of_base_class() => An_issue_is_reported_for(@"
using System;

public class TestMeBase
{
    public bool Ordinal { get; set; }
}

public class TestMe : TestMeBase
{
    public void DoStuff(StringComparison value)
    {
        switch (value)
        {
            case StringComparison.Ordinal:
                Ordinal = true;
                break;

            case StringComparison.OrdinalIgnoreCase:
                Ordinal = true;
                break;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_determines_single_condition_and_keeps_information_in_same_field() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool ordinal;

    public void DoStuff(StringComparison value)
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
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_assigns_to_field_when_method_also_has_local_variable() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private bool ordinal;

    public void DoStuff(StringComparison value)
    {
        var x = 42;

        switch (value)
        {
            case StringComparison.Ordinal:
                ordinal = true;
                break;

            case StringComparison.OrdinalIgnoreCase:
                ordinal = true;
                break;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_that_assigns_to_out_parameter_when_method_also_has_local_variable() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoStuff(StringComparison value, out bool ordinal)
    {
        var x = 42;

        switch (value)
        {
            case StringComparison.Ordinal:
                ordinal = true;
                break;

            case StringComparison.OrdinalIgnoreCase:
                ordinal = true;
                break;
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_switch_in_top_level_statement_that_assigns_to_variable_inside_local_function() => An_issue_is_reported_for(@"
using System;

object DoCreate(StringComparison value)
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
", languageVersion: LanguageVersion.CSharp9);

        protected override string GetDiagnosticId() => MiKo_3080_SwitchReturnInsteadSwitchBreakAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3080_SwitchReturnInsteadSwitchBreakAnalyzer();
    }
}
