using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_object_creation_with_simple_arguments_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int count) => " + invocation + @"(new string('x', count));
}
");

        [Test]
        public void An_issue_is_reported_for_String_invocation_with_object_creation_with_simple_arguments_and_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int count, string paramName) => " + invocation + @"(new string('x', count), paramName);
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_String_invocation_with_target_typed_object_creation_with_simple_arguments_and_no_paramName_([ValueSource(nameof(NullChecks))] string invocation) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int count) => " + invocation + @"(new('x', count));
}
", LanguageVersion.CSharp9);
    }
}
