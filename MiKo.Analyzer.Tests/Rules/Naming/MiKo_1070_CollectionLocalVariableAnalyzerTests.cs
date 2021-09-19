using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1070_CollectionLocalVariableAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
            {
                "map",
                "array",
                "buffer",
                "collection",
                "dictionary",
                "items",
                "list",
                "queue",
                "result",
                "results",
                "source",
                "stack",
                "typesUnderTest",
                "variablesRead",
                "variablesWritten",
                "gateways",
            };

        private static readonly string[] WrongNames =
            {
                "item",
                "enumerable",
                "target",
                "myDictionary",
                "myList42",
            };

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_non_Collection_variable() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
        int i = 0;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_Collection_variable_with_correct_name_([ValueSource(nameof(CorrectNames))] string name) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        int[] " + name + @" = new int[0];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_var_Collection_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        var items = new int[0];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_byte_array_for_hash() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        byte[] hash = new byte[0];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_IGrouping() => No_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        IGrouping<string, string> group = null;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_Collection_variable_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        int[] " + name + @" = new int[0];
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_var_Collection_variable_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        var " + name + @" = new int[0];
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_variable_declaration_pattern_for_Collection_variable_with_correct_name() => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int[] items: return;
            default: return;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_variable_declaration_pattern_for_Collection_variable_with_incorrect_name() => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int[] item: return;
            default: return;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_for_statement_without_identifier() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i, string methodName)
    {
        for (; i < methodName.Length; i++)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_strange_ref_usage() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(Guid guid1, Guid guid2)
    {
        ref var x = (guid1 == Guid.Empty) ? ref guid1 : ref guid2;
    }
}
");

        [Test]
        public void Code_gets_fixed_for_variable()
        {
            const string OriginalCode = @"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        int[] number = new int[0];
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething()
    {
        int[] numbers = new int[0];
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_variable_declaration_pattern()
        {
            const string OriginalCode = @"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int[] item: return;
            default: return;
        }
    }
}
";

            const string FixedCode = @"
using System;
using System.Threading;

public class TestMe
{
    public void DoSomething(object o)
    {
        switch (o)
        {
            case int[] items: return;
            default: return;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_1070_CollectionLocalVariableAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1070_CollectionLocalVariableAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1070_CodeFixProvider();
    }
}