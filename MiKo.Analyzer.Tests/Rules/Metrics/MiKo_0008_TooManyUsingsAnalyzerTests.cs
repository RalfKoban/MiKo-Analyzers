using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [TestFixture]
    public sealed class MiKo_0008_TooManyUsingsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_no_using_directives_in_compilation_unit() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_no_using_directives_in_file_scoped_namespace() => No_issue_is_reported_for(@"
namespace Bla;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_no_using_directives_in_namespace() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_at_limit_in_compilation_unit() => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_at_limit_in_file_scoped_namespace() => No_issue_is_reported_for(@"
namespace Bla;

using System;
using System.Collections;
using System.Collections.Generic;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_at_limit_in_namespace() => No_issue_is_reported_for(@"
namespace Bla
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_at_limit_in_compilation_unit_and_additional_using_alias() => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

using Integer32 = System.Int32;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_at_limit_in_file_scoped_namespace_and_additional_using_alias() => No_issue_is_reported_for(@"
namespace Bla;

using System;
using System.Collections;
using System.Collections.Generic;

using Integer32 = System.Int32;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_at_limit_in_namespace_and_additional_using_alias() => No_issue_is_reported_for(@"
namespace Bla
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Integer32 = System.Int32;

    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_below_limit_in_compilation_unit() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_below_limit_in_file_scoped_namespace() => No_issue_is_reported_for(@"
namespace Bla;

using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_using_directives_below_limit_in_namespace() => No_issue_is_reported_for(@"
namespace Bla
{
    using System;

    public class TestMe
    {
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_multiple_using_directives_above_limit_in_compilation_unit() => An_issue_is_reported_for(3, @"
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Math;
using System.Text;

public class TestMe
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_multiple_using_directives_above_limit_in_file_scoped_namespace() => An_issue_is_reported_for(3, @"
namespace Bla;

using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Math;
using System.Text;

public class TestMe
{
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_multiple_using_directives_above_limit_in_namespace() => An_issue_is_reported_for(3, @"
namespace Bla
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using System.IO;
    using System.Math;
    using System.Text;

    public class TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_using_directives_above_limit_in_compilation_unit() => An_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_using_directives_above_limit_in_file_scoped_namespace() => An_issue_is_reported_for(@"
namespace Bla;

using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_using_directives_above_limit_in_namespace() => An_issue_is_reported_for(@"
namespace Bla
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using System.IO;

    public class TestMe
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_0008_TooManyUsingsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_0008_TooManyUsingsAnalyzer { AllowedUsings = 3 };
    }
}