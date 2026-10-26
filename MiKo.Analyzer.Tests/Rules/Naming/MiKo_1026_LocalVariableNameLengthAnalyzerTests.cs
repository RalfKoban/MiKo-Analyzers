using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1026_LocalVariableNameLengthAnalyzerTests : NamingLengthAnalyzerTests
    {
        private static readonly string[] Fitting = GetAllWithMaxLengthOf(Constants.MaxNamingLengths.LocalVariables);
        private static readonly string[] NonFitting = GetAllAboveLengthOf(Constants.MaxNamingLengths.LocalVariables);

        [Test]
        public void No_issue_is_reported_for_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        var " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        var " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        const int " + name + @" = 42;
        return " + name + @"; 
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_declaration_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return 42;
            default: return -1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_declaration_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(object o)
    {
        switch (o)
        {
            case int " + name + @": return 42;
            default: return -1;
        }
    }
}
");

        [Test] // shall be covered by MiKo_1027
        public void No_issue_is_reported_for_variable_1_in_tuple_deconstruction_in_foreach_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
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

        [Test] // shall be covered by MiKo_1027
        public void No_issue_is_reported_for_variable_2_in_tuple_deconstruction_in_foreach_variable_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
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

        [Test] // shall be covered by MiKo_1027
        public void No_issue_is_reported_for_variable_1_in_tuple_deconstruction_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
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

        [Test] // shall be covered by MiKo_1027
        public void No_issue_is_reported_for_variable_2_in_tuple_deconstruction_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
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

        [Test] // shall be covered by MiKo_1027
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

        [Test] // shall be covered by MiKo_1027
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

        [Test] // shall be covered by MiKo_1027
        public void No_issue_is_reported_for_variable_declaration_1_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
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

        [Test] // shall be covered by MiKo_1027
        public void No_issue_is_reported_for_variable_declaration_2_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
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

        [Test] // shall be covered by MiKo_1027
        public void No_issue_is_reported_for_variable_in_nested_tuple_deconstruction_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
using System.Collections.Generic;

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

        [Test] // shall be covered by MiKo_1027
        public void No_issue_is_reported_for_variable_in_nested_variable_designation_in_foreach_variable_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => No_issue_is_reported_for(@"
using System.Collections.Generic;

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
        public void An_issue_is_reported_for_variable_in_foreach_body_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{   
    public void DoSomething((int, (int, int))[] data)
    {
        foreach (var (key, (value, other)) in data)
        {
            var (" + name + @", x) = (1, 2);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_1_in_tuple_deconstruction_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int> pair)
    {
        var (" + name + @", value) = pair;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_2_in_tuple_deconstruction_with_fitting_length_([ValueSource(nameof(Fitting))] string name) => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int> pair)
    {
        var (key, " + name + @") = pair;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_1_in_tuple_deconstruction_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int> pair)
    {
        var (" + name + @", value) = pair;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_2_in_tuple_deconstruction_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int> pair)
    {
        var (key, " + name + @") = pair;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_typed_variable_1_in_tuple_deconstruction_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int> pair)
    {
        (int " + name + @", int value) = pair;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_typed_variable_2_in_tuple_deconstruction_with_exceeding_length_([ValueSource(nameof(NonFitting))] string name) => An_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(KeyValuePair<int, int> pair)
    {
        (int key, int " + name + @") = pair;
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1026_LocalVariableNameLengthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest()
        {
            NamingLengthAnalyzer.EnabledPerDefault = true;

            Analyzer.Reset();

            return new MiKo_1026_LocalVariableNameLengthAnalyzer();
        }
    }
}