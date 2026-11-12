using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3016_ArgumentNullExceptionThrownAtWrongPlaceAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_ArgumentException_ThrowIfNullOrEmpty_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentException.ThrowIfNullOrEmpty(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentException_ThrowIfNullOrWhiteSpace_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_ThrowIfNull_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentNullException.ThrowIfNull(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_ThrowIfNullOrEmpty_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentNullException_ThrowIfNullOrWhiteSpace_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string s)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfEqual_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfGreaterThan_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfGreaterThanOrEqual_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfLessThan_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfLessThanOrEqual_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNegative_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(i);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNegativeOrZero_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(i);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfNotEqual_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(i, 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ArgumentOutOfRangeException_ThrowIfZero_on_parameter() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        ArgumentOutOfRangeException.ThrowIfZero(i);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_InvalidEnumArgumentException_ThrowIfNullOrEmpty_on_parameter() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(string s)
    {
        InvalidEnumArgumentException.ThrowIfNullOrEmpty(s);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_InvalidEnumArgumentException_ThrowIfNullOrWhiteSpace_on_parameter() => No_issue_is_reported_for(@"
using System;
using System.ComponentModel;

public class TestMe
{
    public void DoSomething(string s)
    {
        InvalidEnumArgumentException.ThrowIfNullOrWhiteSpace(s);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_on_local_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNull(SomeData);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_on_parameter_property() => An_issue_is_reported_for(@"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        ArgumentNullException.ThrowIfNull(o.SomeData);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNullOrEmpty_on_local_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNullOrEmpty(SomeData);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNullOrEmpty_on_parameter_property() => An_issue_is_reported_for(@"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(o.SomeData);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNullOrWhiteSpace_on_local_property() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(SomeData);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNullOrWhiteSpace_on_parameter_property() => An_issue_is_reported_for(@"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(o.SomeData);
    }
}
");

        [Test]
        public void Code_gets_fixed_for_ThrowIfNull_on_conditional_access_of_parameter_property()
        {
            const string OriginalCode = @"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        ArgumentNullException.ThrowIfNull(o?.SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (o?.SomeData is null)
        {
            throw new ArgumentException(""TODO"", nameof(o));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ThrowIfNull_on_local_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNull(SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        if (SomeData is null)
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ThrowIfNull_on_parameter_property()
        {
            const string OriginalCode = @"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        ArgumentNullException.ThrowIfNull(o.SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (o.SomeData is null)
        {
            throw new ArgumentException(""TODO"", nameof(o));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ThrowIfNullOrEmpty_on_local_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNullOrEmpty(SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        if (string.IsNullOrEmpty(SomeData))
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ThrowIfNullOrEmpty_on_parameter_property()
        {
            const string OriginalCode = @"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(o.SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (string.IsNullOrEmpty(o.SomeData))
        {
            throw new ArgumentException(""TODO"", nameof(o));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ThrowIfNullOrWhiteSpace_on_local_property()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public string SomeData { get; set; }

    public void DoSomething()
    {
        if (string.IsNullOrWhiteSpace(SomeData))
        {
            throw new InvalidOperationException(""TODO"");
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ThrowIfNullOrWhiteSpace_on_parameter_property()
        {
            const string OriginalCode = @"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(o.SomeData);
    }
}
";

            const string FixedCode = @"
using System;

public class SomeObject
{
    public string SomeData { get; set; }
}

public class TestMe
{
    public void DoSomething(SomeObject o)
    {
        if (string.IsNullOrWhiteSpace(o.SomeData))
        {
            throw new ArgumentException(""TODO"", nameof(o));
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}