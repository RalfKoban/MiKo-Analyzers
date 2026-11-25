using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_method_inside_object_creation_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => ArgumentNullException.ThrowIfNull(new Action(delegate { }));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_method_inside_object_creation_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string paramName) => ArgumentNullException.ThrowIfNull(new Action(delegate { }), paramName);
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_method_with_parameter_list_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => ArgumentNullException.ThrowIfNull(new Func<string, string>(delegate (string x) { return x; }));
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_method_inside_cast_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => ArgumentNullException.ThrowIfNull((Action)delegate { });
}
");

        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_method_inside_parenthesized_object_creation_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => ArgumentNullException.ThrowIfNull((new Action(delegate { })));
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test] // use the anonymous method directly as argument (which needs a natural type), as otherwise a surrounding node is reported first
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_method_as_direct_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => ArgumentNullException.ThrowIfNull(delegate () { });
}
", LanguageVersion.CSharp10);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test] // use the anonymous method directly as argument (which needs a natural type), as otherwise a surrounding node is reported first
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_method_as_direct_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(string paramName) => ArgumentNullException.ThrowIfNull(delegate () { }, paramName);
}
", LanguageVersion.CSharp10);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test] // use the anonymous method directly as argument (which needs a natural type), as otherwise a surrounding node is reported first
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_anonymous_method_with_parameter_list_as_direct_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => ArgumentNullException.ThrowIfNull(delegate (string x) { return x; });
}
", LanguageVersion.CSharp10);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test] // use the anonymous method directly as argument (which needs a natural type), as otherwise a surrounding node is reported first
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_anonymous_method_as_direct_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() => ArgumentNullException.ThrowIfNull((delegate () { }));
}
", LanguageVersion.CSharp10);
    }
}
