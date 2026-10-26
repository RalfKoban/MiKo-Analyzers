using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.LocalVariablesInLoops);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.LocalVariablesInLoops);

        [Test]
        public void No_issue_is_reported_for_variable_in_foreach_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(string[] data)
    {
        foreach (var " + name + @" in data)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_in_for_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        for (var " + name + " = 0; " + name + " < 1; " + name + @"++)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_tuple_1_in_foreach_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int>[] data)
    {
        foreach ((int " + name + @", int value) in data)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_tuple_2_in_foreach_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int>[] data)
    {
        foreach ((int key, int " + name + @") in data)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_1_in_foreach_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int>[] data)
    {
        foreach (var (" + name + @", value) in data)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_2_in_foreach_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int>[] data)
    {
        foreach (var (key, " + name + @") in data)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_in_foreach_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(string[] data)
    {
        foreach (var " + name + @" in data)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_in_for_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        for (var " + name + " = 0; " + name + " < 1; " + name + @"++)
        {
        }

    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_tuple_1_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int>[] data)
    {
        foreach ((int " + name + @", int value) in data)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_tuple_2_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int>[] data)
    {
        foreach ((int key, int " + name + @") in data)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_1_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int>[] data)
    {
        foreach (var (" + name + @", value) in data)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_2_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int>[] data)
    {
        foreach (var (key, " + name + @") in data)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_in_nested_tuple_in_foreach_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething((int, (int, int))[] data)
    {
        foreach ((int key, (int value, int " + name + @")) in data)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_in_nested_declaration_in_foreach_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething((int, (int, int))[] data)
    {
        foreach (var (key, (value, " + name + @")) in data)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_in_nested_tuple_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething((int, (int, int))[] data)
    {
        foreach ((int key, (int value, int " + name + @")) in data)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_in_nested_declaration_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething((int, (int, int))[] data)
    {
        foreach (var (key, (value, " + name + @")) in data)
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            NamingLengthAnalyzer.EnabledPerDefault = true;

            Analyzer.Reset();

            return new MiKo_1027_LocalVariableNameInForLoopsLengthAnalyzer();
        }
    }
}