using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3229_KeyValuePairCreateAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_argument() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        yield return null;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_usage_on_return() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        yield return KeyValuePair.Create(""some key"", ""some value"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrect_usage_on_return() => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        yield return new KeyValuePair<string, string>(""some key"", ""some value"");
    }
}
");

        [Test]
        public void Code_gets_fixed_for_incorrect_usage_on_return()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        yield return new KeyValuePair<string, string>(""some key"", ""some value"");
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        yield return KeyValuePair.Create(""some key"", ""some value"");
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrect_usage_on_return_array()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        return
               [
                   new KeyValuePair<string, string>(""some key 1"", ""some value 1""),
                   new KeyValuePair<string, string>(""some key 2"", ""some value 2""),
               ];
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        return
               [
                   KeyValuePair.Create(""some key 1"", ""some value 1""),
                   KeyValuePair.Create(""some key 2"", ""some value 2""),
               ];
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrect_usage_on_return_array_with_comment()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        return
               [
                   new KeyValuePair<string, string>(""some key 1"", ""some value 1"") /* some comment 1 */,
                   new KeyValuePair<string, string>(""some key 2"", ""some value 2"") /* some comment 2 */,
               ];
    }
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething()
    {
        return
               [
                   KeyValuePair.Create(""some key 1"", ""some value 1"") /* some comment 1 */,
                   KeyValuePair.Create(""some key 2"", ""some value 2"") /* some comment 2 */,
               ];
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrect_usage_on_arrow_clause_with_array()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething() =>
                                                                     [
                                                                         new KeyValuePair<string, string>(""some key 1"", ""some value 1""),
                                                                         new KeyValuePair<string, string>(""some key 2"", ""some value 2""),
                                                                     ];
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething() =>
                                                                     [
                                                                         KeyValuePair.Create(""some key 1"", ""some value 1""),
                                                                         KeyValuePair.Create(""some key 2"", ""some value 2""),
                                                                     ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_incorrect_usage_on_arrow_clause_with_array_and_comment()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething() =>
                                                                     [
                                                                         new KeyValuePair<string, string>(""some key 1"", ""some value 1"") /* some comment 1 */,
                                                                         new KeyValuePair<string, string>(""some key 2"", ""some value 2"") /* some comment 2 */,
                                                                     ];
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMe
{
    public IEnumerable<KeyValuePair<string,string>> DoSomething() =>
                                                                     [
                                                                         KeyValuePair.Create(""some key 1"", ""some value 1"") /* some comment 1 */,
                                                                         KeyValuePair.Create(""some key 2"", ""some value 2"") /* some comment 2 */,
                                                                     ];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3229_KeyValuePairCreateAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3229_KeyValuePairCreateAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3229_CodeFixProvider();
    }
}