﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6052_BaseListOperatorsAreOnSameLineAsBaseListTypeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_type_without_base_list() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_incomplete_base_list_when_colon_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe :
{
}
");

        [Test]
        public void No_issue_is_reported_for_incomplete_base_list_when_colon_is_on_other_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
                :
{
}
");

        [Test]
        public void No_issue_is_reported_for_base_list_when_colon_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe : IDisposable
{
}
");

        [Test]
        public void No_issue_is_reported_for_base_list_when_colon_is_on_same_line_but_on_another_as_type_itself() => No_issue_is_reported_for(@"
using System;

public class TestMe
                : IDisposable
{
}
");

        [Test]
        public void An_issue_is_reported_for_base_list_when_colon_is_on_other_line_than_first_base_type() => An_issue_is_reported_for(@"
using System;

public class TestMe :
                IDisposable
{
}
");

        [Test]
        public void No_issue_is_reported_for_generic_types_base_list_when_colon_is_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe<T> : IDisposable
{
}
");

        [Test]
        public void No_issue_is_reported_for_generic_types_base_list_when_colon_is_on_same_line_but_on_another_as_type_itself() => No_issue_is_reported_for(@"
using System;

public class TestMe<T>
                : IDisposable
{
}
");

        [Test]
        public void An_issue_is_reported_for_generic_base_list_when_colon_is_on_other_line_than_first_base_type() => An_issue_is_reported_for(@"
using System;

public class TestMe<T> :
                    IDisposable
{
}
");

        [Test]
        public void Code_gets_fixed_for_non_generic_([Values("class", "interface", "struct", "record")] string typeKind)
        {
            const string OriginalCode = @"
using System;

public ### TestMe :
                IDisposable
{
}
";

            const string FixedCode = @"
using System;

public ### TestMe
                : IDisposable
{
}
"
            ;
            VerifyCSharpFix(OriginalCode.Replace("###", typeKind), FixedCode.Replace("###", typeKind));
        }

        [Test]
        public void Code_gets_fixed_for_generic_([Values("class", "interface", "struct", "record")] string typeKind)
        {
            const string OriginalCode = @"
using System;

public ### TestMe<T> :
                IDisposable
{
}
";

            const string FixedCode = @"
using System;

public ### TestMe<T>
                : IDisposable
{
}
";

            VerifyCSharpFix(OriginalCode.Replace("###", typeKind), FixedCode.Replace("###", typeKind));
        }

        protected override string GetDiagnosticId() => MiKo_6052_BaseListOperatorsAreOnSameLineAsBaseListTypeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6052_BaseListOperatorsAreOnSameLineAsBaseListTypeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6052_CodeFixProvider();
    }
}