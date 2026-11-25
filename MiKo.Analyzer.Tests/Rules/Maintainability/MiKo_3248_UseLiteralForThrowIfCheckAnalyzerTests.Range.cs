using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_range_slice_with_start_only_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => " + invocation + @"(value[1..]);
}
", LanguageVersion.CSharp8);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_range_slice_with_start_only_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, string paramName) => " + invocation + @"(value[1..], paramName);
}
", LanguageVersion.CSharp8);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_range_slice_with_start_and_end_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => " + invocation + @"(value[1..3]);
}
", LanguageVersion.CSharp8);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_range_slice_with_end_only_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => " + invocation + @"(value[..3]);
}
", LanguageVersion.CSharp8);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_range_slice_with_variable_bounds_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value, int start, int end) => " + invocation + @"(value[start..end]);
}
", LanguageVersion.CSharp8);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_parenthesized_range_slice_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string value) => " + invocation + @"((value[1..]));
}
", LanguageVersion.CSharp8);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_array_range_slice_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string[] values) => ArgumentNullException.ThrowIfNull(values[1..]);
}
", LanguageVersion.CSharp8);
    }
}
